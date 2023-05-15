"use strict";

function main() {
    let gameStartTime = new Date();
    let speedArray = [];
    let quizStartTime = null;
    let quiz = "";
    let wrongAnswer = false;

    const statsElement = document.querySelector(".stats");
    const questionElement = document.querySelector(".question");
    const answerElement = document.querySelector(".answer");

    document.addEventListener("keydown", sendAnswer);

    function drawNewQuiz() {
        quiz = quizList.shift();
        quizStartTime = new Date();

        questionElement.textContent = quiz;
        answerElement.value = "";
    }

    function sendAnswer(evt) {
        if (evt.key !== "Enter") {
            return;
        }

        wrongAnswer = answerElement.value !== quiz;

        const timeLength = new Date() - quizStartTime;
        const quizLength = Hangul.d(quiz).length;
        speedArray.push(Math.round((60000 / timeLength) * quizLength));

        showStats();
        drawNewQuiz();
    }

    function showStats() {
        const totalTime = Math.round((new Date() - gameStartTime) / 1000);
        const sec = totalTime % 60;
        const min = (totalTime - sec) / 60;
        const speedAvg = Math.round(
            speedArray.reduce((prev, current) => prev + current, 0) / speedArray.length
        );
        statsElement.textContent = `시간: ${min.toString().padStart(2, "0")}:${sec
            .toString()
            .padStart(2, "0")} / 마지막: ${
            speedArray[speedArray.length - 1] || 0
        }타, 평균: ${speedAvg || 0}타${wrongAnswer ? " (오답!!)" : ""}`;
    }

    drawNewQuiz();
    showStats();
    setInterval(showStats, 1000);
}

main();
