function encrypt(text) {
    let parts = Hangul.disassemble(text, true);
    let result = [];
    let encrypt = "";
    const list = [
        [
            "ㄱ",
            "ㄲ",
            "ㄴ",
            "ㄷ",
            "ㄸ",
            "ㄹ",
            "ㅁ",
            "ㅂ",
            "ㅃ",
            "ㅅ",
            "ㅆ",
            "ㅇ",
            "ㅈ",
            "ㅉ",
            "ㅊ",
            "ㅋ",
            "ㅌ",
            "ㅍ",
            "ㅎ"
        ],

        [
            "ㅏ",
            "ㅐ",
            "ㅑ",
            "ㅒ",
            "ㅓ",
            "ㅔ",
            "ㅕ",
            "ㅖ",
            "ㅗ",
            "ㅘ",
            "ㅙ",
            "ㅚ",
            "ㅛ",
            "ㅜ",
            "ㅝ",
            "ㅞ",
            "ㅟ",
            "ㅠ",
            "ㅡ",
            "ㅢ",
            "ㅣ"
        ],

        [
            "ㄱ",
            "ㄲ",
            "ㄳ",
            "ㄴ",
            "ㄵ",
            "ㄶ",
            "ㄷ",
            "ㄹ",
            "ㄺ",
            "ㄻ",
            "ㄼ",
            "ㄽ",
            "ㄾ",
            "ㄿ",
            "ㅀ",
            "ㅁ",
            "ㅂ",
            "ㅄ",
            "ㅅ",
            "ㅆ",
            "ㅇ",
            "ㅈ",
            "ㅊ",
            "ㅋ",
            "ㅌ",
            "ㅍ",
            "ㅎ"
        ]
    ];

    for (let item of parts) {
        let word = [];

        let consonant = 0;
        let vowel = 0;

        for (let i = 0; i < item.length; i++) {
            if (Hangul.isConsonant(item[i])) {
                consonant += 1;
            } else {
                vowel += 1;
            }
        }

        if (vowel == 2) {
            item[1] = Hangul.assemble([item[1], item[2]]);
            item[2] = undefined;
        }

        if (consonant === 3) {
            item[item.length - 2] = Hangul.assemble([
                item[item.length - 2],
                item[item.length - 1]
            ]);
            item[item.length - 1] = undefined;
        }

        item = item.filter((item) => item !== undefined);

        for (let i = 0; i < item.length; i++) {
            let index = list[i].indexOf(item[i]) - 1;
            if (index < 0) {
                index += list[i].length;
            } else if (index >= list[i].length) {
                index -= list[i].length;
            }
            word.push(list[i][index]);
        }
        result.push(word);
    }

    for (let item of result) {
        encrypt += Hangul.assemble(item);
    }

    console.log(`원문: ${text}, 암호문: ${encrypt}`);
}

function decrypt(text) {
    let parts = Hangul.disassemble(text, true);
    let result = [];
    let encrypt = "";
    const list = [
        [
            "ㄱ",
            "ㄲ",
            "ㄴ",
            "ㄷ",
            "ㄸ",
            "ㄹ",
            "ㅁ",
            "ㅂ",
            "ㅃ",
            "ㅅ",
            "ㅆ",
            "ㅇ",
            "ㅈ",
            "ㅉ",
            "ㅊ",
            "ㅋ",
            "ㅌ",
            "ㅍ",
            "ㅎ"
        ],

        [
            "ㅏ",
            "ㅐ",
            "ㅑ",
            "ㅒ",
            "ㅓ",
            "ㅔ",
            "ㅕ",
            "ㅖ",
            "ㅗ",
            "ㅘ",
            "ㅙ",
            "ㅚ",
            "ㅛ",
            "ㅜ",
            "ㅝ",
            "ㅞ",
            "ㅟ",
            "ㅠ",
            "ㅡ",
            "ㅢ",
            "ㅣ"
        ],

        [
            "ㄱ",
            "ㄲ",
            "ㄳ",
            "ㄴ",
            "ㄵ",
            "ㄶ",
            "ㄷ",
            "ㄹ",
            "ㄺ",
            "ㄻ",
            "ㄼ",
            "ㄽ",
            "ㄾ",
            "ㄿ",
            "ㅀ",
            "ㅁ",
            "ㅂ",
            "ㅄ",
            "ㅅ",
            "ㅆ",
            "ㅇ",
            "ㅈ",
            "ㅊ",
            "ㅋ",
            "ㅌ",
            "ㅍ",
            "ㅎ"
        ]
    ];

    for (let item of parts) {
        let word = [];

        let consonant = 0;
        let vowel = 0;

        for (let i = 0; i < item.length; i++) {
            if (Hangul.isConsonant(item[i])) {
                consonant += 1;
            } else {
                vowel += 1;
            }
        }

        if (vowel == 2) {
            item[1] = Hangul.assemble([item[1], item[2]]);
            item[2] = undefined;
        }

        if (consonant === 3) {
            item[item.length - 2] = Hangul.assemble([
                item[item.length - 2],
                item[item.length - 1]
            ]);
            item[item.length - 1] = undefined;
        }

        item = item.filter((item) => item !== undefined);

        for (let i = 0; i < item.length; i++) {
            let index = list[i].indexOf(item[i]) + 1;
            if (index < 0) {
                index += list[i].length;
            } else if (index >= list[i].length) {
                index -= list[i].length;
            }
            word.push(list[i][index]);
        }
        result.push(word);
    }

    for (let item of result) {
        encrypt += Hangul.assemble(item);
    }

    console.log(`암호문: ${text}, 원문: ${encrypt}`);
}

function re(text) {
    decrypt(text);
    encrypt(text);
}
