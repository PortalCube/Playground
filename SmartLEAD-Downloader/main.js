"use strict";

const axios = require("axios").default;
const fs = require("fs").promises;
const util = require("util");
const execFile = util.promisify(require("child_process").execFile);
const { Buffer } = require("buffer");

async function download(url, list, data) {
    while (list.length > 0) {
        const item = list.pop();
        const res = await axios.get(url + item, {
            responseType: "arraybuffer"
        });
        await fs.writeFile("download/" + item, res.data);
        console.log(
            `${++data.count} / ${data.total} (${
                Math.round((data.count / data.total) * 10000) / 100
            }%)`
        );
    }
}

async function main(id) {
    const URL = `https://didtculpuhjx4953564.cdn.ntruss.com/hls/${id}/mp4/${id}.mp4/`;
    const indexName = "index.m3u8";

    await fs.rm(`./download/`, { recursive: true, force: true });

    try {
        await fs.mkdir("./download");
    } catch (e) {}
    try {
        await fs.mkdir("./out");
    } catch (e) {}

    const indexFile = (await axios.get(URL + indexName)).data;
    const list = [...indexFile.matchAll(/^(segment-\d{1,4}-v1-a1.ts)$/gm)].map(
        (item) => item[0]
    );

    try {
        await fs.rm(`out/${id}.mp4`);
    } catch (e) {}

    await fs.writeFile(`download/index.m3u8`, indexFile);

    const promises = [];
    const data = {
        total: list.length,
        count: 0
    };

    for (let i = 0; i < 8; i++) {
        promises.push(download(URL, list, data));
    }

    await Promise.all(promises);

    const result = await execFile("ffmpeg", [
        "-i",
        "download/index.m3u8",
        "-bsf:a",
        "aac_adtstoasc",
        "-vcodec",
        "copy",
        "-c",
        "copy",
        "-crf",
        "50",
        `out/${id}.mp4`
    ]);

    console.log("DONE");
}

(() => {
    const arg = process.argv.slice(2);

    if (arg.length == 0) {
        console.error("Usage: node main.js [ID]");
        return;
    } else {
        main(arg[0]);
    }
})();

console.log(process.argv.slice(2));
