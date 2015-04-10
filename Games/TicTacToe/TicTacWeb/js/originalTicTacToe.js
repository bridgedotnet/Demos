// VARIABLES

var human = 'x'; // turn = 0
var computer = 'o'; // turn = 1
var compMove;
var turn = 0; // toggles btw 0 and 1 for switching turns

var boardCheck; // function to check value in each cell
var a1; // value within each cell
var a2;
var a3;
var b1;
var b2;
var b3;
var c1;
var c2;
var c3;

var cellList = ["a1", "a2", "a3", "b1", "b2", "b3", "c1", "c2", "c3"];

var checkWin; // function that checks the board for winning combo
var xWin = false; // true if X wins
var oWin = false; // true if O wins
var winAlert; // function that declares winner and restarts game

var clearBoard;


// Performs a step of the game flow. If click, then check clicked space if allowed.
var gameFlow = function () {
    if ($(this).is("td") && turn == 0 && $(this).text() == "") {
        $(this).text(human);
        boardCheck();
        turn = 1;
        if (!checkWin()) {
            compMove();
            boardCheck();
            checkWin();
        }
    }
};


// Initializes the game flow.
$(document).ready(function () {
    $('#restart').click(clearBoard);
    $('td').click(gameFlow);
    gameFlow();

    $('#gameStatus').css('background', '#bef');
    $('#gameStatus').text('Make your move!');
});

// compMove's basic AI checks if there's a winning sequence, if there's a way to block lose, or randomly picks a cell.
function compMove() {
    if (turn == 0) return;

    if (!fillGap(computer) && !fillGap(human)) {
        // IF NO OPP TO BLOCK A WIN, THEN PLAY IN ONE OF THESE SQUARES
        var freeCellList = [];
        for (var i = 0; i < cellList.length; i++) {
            if ($('#' + cellList[i]).text() == "") freeCellList.push(cellList[i]);
        }

        if (freeCellList.length > 0) {
            var chosenCell = Math.floor(Math.random() * freeCellList.length);
            $('#' + freeCellList[chosenCell]).text(computer);
        }
    }

    // finished turn. next turn will be human's
    turn = 0;
};

function fillGap(checkChar) {
    if (a1 == "" && ((a3 == checkChar && a2 == checkChar) || (c3 == checkChar && b2 == checkChar) || (c1 == checkChar && b1 == checkChar))) {
        $('#a1').text(computer);
        return true;
    }
    else if (a2 == "" && ((a1 == checkChar && a3 == checkChar) || (c2 == checkChar && b2 == checkChar))) {
        $('#a2').text(computer);
        return true;
    }
    else if (a3 == "" && ((a1 == checkChar && a2 == checkChar) || (c1 == checkChar && b2 == checkChar) || (c3 == checkChar && b3 == checkChar))) {
        $('#a3').text(computer);
        return true;
    }
    else if (c3 == "" && ((c1 == checkChar && c2 == checkChar) || (a1 == checkChar && b2 == checkChar) || (a3 == checkChar && b3 == checkChar))) {
        $('#c3').text(computer);
        return true;
    }
    else if (c1 == "" && ((c3 == checkChar && c2 == checkChar) || (a3 == checkChar && b2 == checkChar) || (a1 == checkChar && b1 == checkChar))) {
        $('#c1').text(computer);
        return true;
    }
    else if (c2 == "" && ((c3 == checkChar && c1 == checkChar) || (a2 == checkChar && b2 == checkChar))) {
        $('#c2').text(computer);
        return true;
    }
    else if (b1 == "" && ((b3 == checkChar && b2 == checkChar) || (a1 == checkChar && c1 == checkChar))) {
        $('#b1').text(computer);
        return true;
    }
    else if (b3 == "" && ((a3 == checkChar && c3 == checkChar) || (b2 == checkChar && b1 == checkChar))) {
        $('#b3').text(computer);
        return true;
    }
    else if (b2 == "" && ((a3 == checkChar && c1 == checkChar) || (c3 == checkChar && a1 == checkChar) ||
             (b3 == checkChar && b1 == checkChar) || (c2 == checkChar && a2 == checkChar))) {
        $('#b2').text(computer);
        return true;
    }

    return false;
}

// Tracks checked cells in the game
function boardCheck() {
    a1 = $('#a1').text();
    a2 = $('#a2').text();
    a3 = $('#a3').text();
    b1 = $('#b1').text();
    b2 = $('#b2').text();
    b3 = $('#b3').text();
    c1 = $('#c1').text();
    c2 = $('#c2').text();
    c3 = $('#c3').text();
};

// Checks against a player win, lose or tie
checkWin = function () { // CHECKS IF X WON
    if ((a1 == a2 && a1 == a3 && (a1 == "x")) || //first row
    (b1 == b2 && b1 == b3 && (b1 == "x")) || //second row
    (c1 == c2 && c1 == c3 && (c1 == "x")) || //third row
    (a1 == b1 && a1 == c1 && (a1 == "x")) || //first column
    (a2 == b2 && a2 == c2 && (a2 == "x")) || //second column
    (a3 == b3 && a3 == c3 && (a3 == "x")) || //third column
    (a1 == b2 && a1 == c3 && (a1 == "x")) || //diagonal 1
    (a3 == b2 && a3 == c1 && (a3 == "x")) //diagonal 2
    ) {
        xWin = true;
        winAlert();

    } else { // CHECKS IF O WON
        if ((a1 == a2 && a1 == a3 && (a1 == "o")) || //first row
        (b1 == b2 && b1 == b3 && (b1 == "o")) || //second row
        (c1 == c2 && c1 == c3 && (c1 == "o")) || //third row
        (a1 == b1 && a1 == c1 && (a1 == "o")) || //first column
        (a2 == b2 && a2 == c2 && (a2 == "o")) || //second column
        (a3 == b3 && a3 == c3 && (a3 == "o")) || //third column
        (a1 == b2 && a1 == c3 && (a1 == "o")) || //diagonal 1
        (a3 == b2 && a3 == c1 && (a3 == "o")) //diagonal 2
        ) {
            oWin = true;
            winAlert();

        } else { // CHECKS FOR TIE GAME IF ALL CELLS ARE FILLED
            if (((a1 == "x") || (a1 == "o")) && ((b1 == "x") || (b1 == "o")) && ((c1 == "x") || (c1 == "o")) && ((a2 == "x") || (a2 == "o")) && ((b2 == "x") || (b2 == "o")) && ((c2 == "x") || (c2 == "o")) && ((a3 == "x") || (a3 == "o")) && ((b3 == "x") || (b3 == "o")) && ((c3 == "x") || (c3 == "o"))) {
                $('#gameStatus').css('background', '#eef');
                $('#gameStatus').text("Cat's game!");
                return true;
            }
        }
    }
    return (xWin || oWin);
};


// DECLARES WHO WON
var winAlert = function () {
    if (xWin == true) {
        $('#gameStatus').css('background', '#cfc');
        $('#gameStatus').text('Victory is yours!');
    } else {
        if (oWin == true) {
            $('#gameStatus').css('background', '#fbb');
            $('#gameStatus').text('Better luck next time!');
        }
    }
};


// NEWGAME BUTTON CLEARS THE BOARD, RESTARTS GAME, AND RESETS THE WINS
var clearBoard = function (event) {
    a1 = a2 = a3 = b1 = b2 = b3 = c1 = c2 = c3 = "";
    $('#a1').text("");
    $('#b1').text("");
    $('#c1').text("");
    $('#a2').text("");
    $('#b2').text("");
    $('#c2').text("");
    $('#a3').text("");
    $('#b3').text("");
    $('#c3').text("");
    xWin = false;
    oWin = false;

    $('#gameStatus').css('background', '#bef');
    $('#gameStatus').text('Make your move!');

    gameFlow();
    if (turn == 1) compMove();
};

// STILL NEED TO FIX:
// * Alert for tie game or xWin appears twice
// * X's can replace O's
// * Missed opportunities for O to win
// * Almost never let's human win
// * Clean up logic for compMove