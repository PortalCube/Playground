"use strict";

const { Buffer } = require("buffer");
const fs = require("fs/promises");
const Walk = require("@root/walk");
const path = require("path");

(async function () {
    const dirList = [];
    const dataList = [];

    const list = [];

    await Walk.walk("./pack", async (err, pathname, dirent) => {
        if (err) {
            console.warn("fs stat error for %s: %s", pathname, err.message);
            return;
        }

        if (dirent.isFile()) {
            const data = await fs.readFile(pathname);

            let name = pathname.split("\\");
            name.shift();
            name = name.join("/");

            list.push({
                data,
                dataSize: data.byteLength,
                name,
                nameSize: name.length
            });
        }
    });

    let buf = Buffer.alloc(16);

    buf.write("UnityWebData1.0");

    function writeString(value) {
        const buf = Buffer.from(value);
        add(buf);
    }

    function add(value) {
        buf = Buffer.concat([buf, value]);
    }

    function writeInt(value) {
        const buf = Buffer.alloc(4);
        buf.writeInt32LE(value);
        add(buf);
    }

    // get Index Info

    // UnityWebData1.0 + 첫 4바이트 복제
    let offset = 16 + 4;
    let size = 0;

    for (const item of list) {
        offset += 12 + item.nameSize;
        size += item.dataSize;
    }

    let isFirst = true;

    for (const item of list) {
        if (isFirst) {
            isFirst = false;
            writeInt(offset);
        }

        // 파일 오프셋
        writeInt(offset);

        // 오프셋에 파일 사이즈만큼 추가
        offset += item.dataSize;

        // 파일 사이즈
        writeInt(item.dataSize);

        // 파일 이름의 사이즈
        writeInt(item.nameSize);

        //n바이트 만큼의 파일 이름
        writeString(item.name);
    }

    for (const item of list) {
        add(item.data);
    }
	
    fs.writeFile("./ba2.data.br", buf);
})();
