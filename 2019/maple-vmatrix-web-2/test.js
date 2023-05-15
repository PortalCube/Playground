var cores = [
    [1, 2, 3],
    [1, 3, 2],
    [2, 1, 3],
    [2, 3, 1],
    [3, 1, 2],
    [3, 2, 1],
];

var coreCombinationResult = [];
var goalLength;
var goal = {};

function SetOverlapCount(skill, overlap) {
    goalLength = skill.length * overlap;
    for (i in skill) {
        goal[skill[i]] = overlap;
    }
}

function GetArrayItems(arr, length, callback, oldindexList = []) {
    // array가 ref로 공유되는 특성이 있어서 새로운 Array로 복사
    var indexList = oldindexList.slice();

    // length개 만큼 선택되었을 때..
    if (indexList.length == length) {
        var result = [];
        for (x in indexList) {
            result.push(arr[indexList[x]]);
        }
        callback(result);
        return 0;
    }

    for (x in arr) {
        // 만약 x 가 indexList의 제일 큰 index보다 작으면 넘기기
        if (x <= indexList[indexList.length - 1]) { continue; }

        // indexList에 추가
        var newindexList = indexList.slice();
        newindexList.push(x);

        GetArrayItems(arr, length, callback, newindexList);
    }
}

function SortArrays(arr) {

    // Array.sort()
    // 음수 - a < b  : a-b 순서로 배치
    // 0    - a == b : 변동 x (a-b 순서로 배치)
    // 양수 - a > b  : b-a 순서로 배치

    arr.sort(
        function (a, b) {
            for (x in a) {
                if (a[x] != b[x]) { return (a[x] - b[x]); }
            }
            // 만약 배열의 길이가 다른 경우, 길이가 긴 쪽이 더 큰것으로 간주
            return a.length - b.length;
        }
    );
    console.log(arr);
}

function isEqual(x, y) {
    const ok = Object.keys, tx = typeof x, ty = typeof y;
    return x && y && tx === 'object' && tx === ty ? (
        ok(x).length === ok(y).length &&
        ok(x).every(key => isEqual(x[key], y[key]))
    ) : (x === y);
}

function CheckCore(arr) {
    var item = [];
    var sum = {};

    for (x in arr) {
        // 첫번째 스킬은 "중복 불가"
        if (item.indexOf(arr[x][0]) != -1) { return; }
        item.push(arr[x][0]);

        // 총 스킬 강화 갯수 세기
        for (i in arr[x]) {
            if (typeof sum[arr[x][i]] === "undefined") { sum[arr[x][i]] = 1; }
            else { sum[arr[x][i]]++; }
        }
    }

    // 각 코어에 들어있는 스킬의 합이 목표치에 미달시 다음 코어로
    if (goalLength % 3 != 0) {
        if (!Object.keys(goal).every(key => isEqual(goal[key], sum[key]))) { return; }
    } else { if (!isEqual(sum, goal)) { return; } }

    coreCombinationResult.push(arr);
    return;
}

SortArrays(cores);
GetItem(cores, 3, PrintResult);

console.log(coreCombinationResult.length);