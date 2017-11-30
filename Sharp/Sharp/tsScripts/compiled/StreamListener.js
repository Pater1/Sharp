var context = new AudioContext();
var WaveFormat = (function () {
    function WaveFormat() {
    }
    return WaveFormat;
}());
var BoomBox = (function () {
    function BoomBox() {
    }
    return BoomBox;
}());
var AudioFetcher = (function () {
    function AudioFetcher(fetcher, queueLength) {
        if (queueLength === void 0) { queueLength = 3; }
        queueLength = Math.floor(queueLength);
        this.Buffer = new Array();
        this.Fetch = fetcher;
    }
    AudioFetcher.prototype.LastOpen = function () {
        for (var i = 0; i < this.Buffer.length; i++) {
            if (this.Buffer[i] == null) {
                return i;
            }
        }
        return -1;
    };
    AudioFetcher.prototype.IsFull = function () {
        return this.Buffer[0] != null;
    };
    AudioFetcher.prototype.Push = function (toAdd, chain) {
        this.Buffer.push(toAdd);
        if (chain) {
            chain(this.Buffer.pop());
            this.Pull();
        }
    };
    AudioFetcher.prototype.Pull = function (callback) {
        var ret = this.Buffer[this.Buffer.length - 1];
        if (ret == null) {
            this.Fetch(callback);
        }
        else {
            this.Fetch(this.Push);
            if (callback) {
                callback(this.Buffer.pop());
            }
        }
    };
    return AudioFetcher;
}());
var AudioStream = (function () {
    function AudioStream(fetcher) {
        this.Drains = Array();
        for (var i = 0; i < 10; i++) {
            this.Drains.push(this.DrainMaker());
        }
        this.Source = fetcher;
        this.Prime();
    }
    AudioStream.prototype.DrainMaker = function () {
        var ret = context.createBufferSource();
        ret.connect(context.destination);
        //ret.onended = () => {
        //    this.OnEnd();
        //};
        ret.loop = false;
        return ret;
    };
    ;
    AudioStream.prototype.CurIndex = function () {
        var sr = context.sampleRate;
        var t = context.currentTime;
        return Math.floor(t * sr);
        //sr = s/t
        //t*sr = s
    };
    AudioStream.prototype.Prime = function () {
        var _this = this;
        this.Source.Pull(function (ret) {
            if (ret == null) {
                _this.Prime();
            }
            else {
                _this.Start(ret);
            }
        });
    };
    AudioStream.prototype.Start = function (ret) {
        this.Drains[0].buffer = ret;
        this.Drains[0].start(0);
        this.OnEnd(2);
    };
    AudioStream.prototype.OnEnd = function (i) {
        var _this = this;
        i = i % this.Drains.length;
        var drain = this.Drains[i];
        this.Drains[i] = this.DrainMaker();
        this.Drains[i + 1] = this.DrainMaker();
        var timeA = context.currentTime;
        this.Source.Pull(function (ret) {
            _this.Drains[i].buffer = ret;
            var timeDelta = context.currentTime - timeA;
            var timeoutDelay = (ret == null ? 0 : ((ret.getChannelData(0).length / ret.sampleRate) - timeDelta)) * 1000;
            setTimeout(function () {
                _this.Drains[i].start(0);
                var j = i + 2;
                _this.OnEnd(j);
            }, timeoutDelay - 80);
            //let k = i - 2;
            //while (k < 0) k += this.Drains.length;
            //this.Drains[i + 1].buffer = this.SubDivide(this.Drains[k], this.Drains[i]);
        });
    };
    return AudioStream;
}());
var key = document.getElementById('data-key').innerText.replace(/\s/g, '');
var minSample = 75000;
var channels = 2;
var fetcher = new AudioFetcher(PullSongChunk, 10);
var stream = new AudioStream(fetcher);
function ProccessSongChunk(chunk) {
    var audio = context.createBuffer(chunk.Format.Channels, chunk.AudioChunk.length / chunk.Format.Channels, chunk.Format.SampleRate);
    for (var i = 0; i < chunk.AudioChunk.length; i++) {
        var c = i % audio.numberOfChannels;
        var l = Math.floor(i / audio.numberOfChannels);
        audio.getChannelData(c)[l] = chunk.AudioChunk[i];
    }
    return audio;
}
//function ProccessSongChunk(chunk: BoomBox): AudioBuffer {
//    let audio: AudioBuffer = context.createBuffer(
//        chunk.Format.Channels,
//        chunk.AudioChunk.length / chunk.Format.Channels,
//        chunk.Format.SampleRate
//    );
//    channels = chunk.Format.Channels;
//    for (let i: number = 0; i < audio.numberOfChannels; i++) {
//        let ar: Float32Array = new Float32Array(chunk.AudioChunk[i]);
//        audio.copyToChannel(ar, i);
//    }
//    return audio;
//}
function PullSongChunk(callback) {
    var xmlHttp = new XMLHttpRequest();
    var url = "/API/PullChunk/" + key + "/" + minSample + "/" + channels;
    xmlHttp.onreadystatechange = function () {
        if (xmlHttp.readyState == 4 && xmlHttp.status == 200) {
            var ret = JSON.parse(xmlHttp.responseText);
            if (ret.Succeeded && ret.AudioChunk != null) {
                console.log(ret);
                callback(ProccessSongChunk(ret));
            }
        }
    };
    xmlHttp.open("GET", url, true); // true for asynchronous 
    xmlHttp.send(null);
}
//# sourceMappingURL=StreamListener.js.map