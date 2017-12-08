let context: AudioContext = new AudioContext();

class WaveFormat {
    BlockAlign: number;
    AverageBytesPerSecond: number;
    SampleRate: number;
    Channels: number;
    Encoding: number;
    ExtraSize: number;
    BitsPerSample: number;
}

class BoomBox {
    Succeeded: boolean;
    AudioChunk: Float32Array;
    ReadPosition: number;

    RequestWaveFormat: boolean;
    Format: WaveFormat; //usually null to save bandwidth
}

class AudioFetcher {
    //Push to 0, Pull from queueLength
    private Buffer: AudioBuffer[];
    constructor(fetcher: (AudioBuffer) => void, queueLength: number = 3) {
        queueLength = Math.floor(queueLength);
        this.Buffer = new Array<AudioBuffer>();

        this.Fetch = fetcher;
    }

    private LastOpen(): number {
        for (let i: number = 0; i < this.Buffer.length; i++) {
            if (this.Buffer[i] == null) {
                return i;
            }
        }
        return -1;
    }

    IsFull(): boolean {
        return this.Buffer[0] != null;
    }
    private Fetch: (AudioBuffer) => void;
    private Push(toAdd: AudioBuffer, chain?: (AudioBuffer) => void) {
        if (toAdd != null) {
            this.Buffer.push(toAdd);
        }
        if (chain) {
            chain(this.Buffer.pop());
            this.Pull();
        }
    }

    Pull(callback?: (AudioBuffer) => void): void {
        let ret: AudioBuffer = this.Buffer[this.Buffer.length - 1];

        if (ret == null) {
            this.Fetch(callback);
        }else{
            this.Fetch(this.Push);
            if (callback) {
                callback(this.Buffer.pop());
            }
        }
    }
}

class AudioStream {
    Source: AudioFetcher;
    Drains: Array<AudioBufferSourceNode>;
    DrainMaker(): AudioBufferSourceNode{
        let ret: AudioBufferSourceNode = context.createBufferSource();
        ret.connect(context.destination);
        
        //ret.onended = () => {
        //    this.OnEnd();
        //};
        ret.loop = false;

        return ret;
    };
    constructor(fetcher: AudioFetcher) {
        this.Drains = Array<AudioBufferSourceNode>();
        for (let i = 0; i < 10; i++) {
            this.Drains.push(this.DrainMaker());
        }
        this.Source = fetcher;
        this.Prime();
    }

    private CurIndex(): number{
        let sr = context.sampleRate;
        let t = context.currentTime;
        return Math.floor(t * sr);
        //sr = s/t
        //t*sr = s
    }

    Prime() {
        this.Source.Pull((ret: AudioBuffer) => {
            if (ret == null) {
                this.Prime();
            } else {
                this.Start(ret);
            }
        });
    }
    
    Start(ret: AudioBuffer) {
        this.Drains[0].buffer = ret;
        this.Drains[0].start(0);

        this.OnEnd(2);
    }
    
    OnEnd(i: number) {
        i = i % this.Drains.length;
        let drain: AudioBufferSourceNode = this.Drains[i];

        this.Drains[i] = this.DrainMaker();
        this.Drains[i+1] = this.DrainMaker();
        let timeA = context.currentTime;
        this.Source.Pull((ret: AudioBuffer) => {
            this.Drains[i].buffer = ret;
            
            let timeDelta = context.currentTime - timeA;
            let timeoutDelay = (ret == null ? 0 : ((ret.getChannelData(0).length / ret.sampleRate) - timeDelta)) * 1000;
            setTimeout(() => {
                this.Drains[i].start(0);
                let j = i + 2;
                this.OnEnd(j);
            }, timeoutDelay - 80);

            //let k = i - 2;
            //while (k < 0) k += this.Drains.length;
            //this.Drains[i + 1].buffer = this.SubDivide(this.Drains[k], this.Drains[i]);
        });
    }

    //Subdivide(ld: AudioBufferSourceNode, nw: AudioBufferSourceNode): AudioBuffer {
    //    let audio: AudioBuffer = context.createBuffer(
    //        ld.channelCount,
    //        ld.buffer.length,
    //        ld.buffer.sampleRate
    //    );

    //    for (let i: number = 0; i < chunk.AudioChunk.length; i++) {
    //        let c: number = i % audio.numberOfChannels;
    //        let l: number = Math.floor(i / audio.numberOfChannels);

    //        audio.getChannelData(c)[l] = chunk.AudioChunk[i];
    //    }

    //    return audio;
    //}
}

let key: string = document.getElementById('data-key').innerText.replace(/\s/g, '');

let minSample: number = 75000;
let channels: number = 2;
let fetcher: AudioFetcher = new AudioFetcher(PullSongChunk, 10);

let stream: AudioStream = new AudioStream(fetcher);


function ProccessSongChunk(chunk: BoomBox): AudioBuffer {
    if (chunk == null || chunk.Format == null || chunk.AudioChunk == null || chunk.AudioChunk.length <= 0) {
        return null;
    }
    let audio: AudioBuffer = context.createBuffer(
        chunk.Format.Channels,
        chunk.AudioChunk.length / chunk.Format.Channels,
        chunk.Format.SampleRate
    );

    for (let i: number = 0; i < chunk.AudioChunk.length; i++) {
        let c: number = i % audio.numberOfChannels;
        let l: number = Math.floor(i / audio.numberOfChannels);

        audio.getChannelData(c)[l] = chunk.AudioChunk[i];
    }

    return audio;
}

function PullSongChunk(callback: (AudioBuffer) => void) {
    let xmlHttp: XMLHttpRequest = new XMLHttpRequest();
    let url: string = "/API/PullChunk/" + key + "/" + minSample + "/" + channels;
    xmlHttp.onreadystatechange = function () {
        if (xmlHttp.readyState == 4 && xmlHttp.status == 200) {
            let ret: BoomBox = JSON.parse(xmlHttp.responseText);
            if (ret.Succeeded && ret.AudioChunk != null) {
                console.log(ret);
                callback(ProccessSongChunk(ret));
            }
        }
    }

    xmlHttp.open("GET", url, true); // true for asynchronous 
    xmlHttp.send(null);
}
