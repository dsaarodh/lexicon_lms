
var svgNS = "http://www.w3.org/2000/svg";

function GetWidth(id) {
    return $("#" + id).innerWidth();
//    console.log(document.getElementById(id).offsetWidth);
    //return document.getElementById(id).offsetWidth;
    //return document.getElementById(id).clientWidth;
}

SVG = {
    createCanvas: function (width, height, containerId) {
        var container = document.getElementById(containerId);
        var canvas = document.createElementNS('http://www.w3.org/2000/svg', 'svg');
        canvas.setAttribute('width', width);
        canvas.setAttribute('height', height);
        container.appendChild(canvas);
        container.setAttribute('height', height);
        container.setAttribute('width', width);
        return canvas;
    },
    createLine: function (x1, y1, x2, y2, color, w) {
        var line = document.createElementNS('http://www.w3.org/2000/svg', 'line');
        line.setAttribute('x1', x1);
        line.setAttribute('y1', y1);
        line.setAttribute('x2', x2);
        line.setAttribute('y2', y2);
        line.setAttribute('stroke', color);
        line.setAttribute('stroke-width', w);
        return line;
    },
    createBox: function (x, y, width, height, fillColor) {
        var rect = document.createElementNS('http://www.w3.org/2000/svg', 'rect');
        rect.setAttributeNS(null, 'x', x);
        rect.setAttributeNS(null, 'y', y);
        rect.setAttributeNS(null, 'height', height);
        rect.setAttributeNS(null, 'width', width);
        rect.setAttributeNS(null, 'fill', fillColor); //'#' + Math.round(0xffffff * Math.random()).toString(16));
        rect.setAttributeNS(null, "stroke", "none");
        return rect;
    },
    createText: function (x, y, text) {
        var textLine = document.createElementNS(svgNS, 'text');
        textLine.setAttribute('x', x);
        textLine.setAttribute('y', y);
        textLine.setAttribute('class', 'timeText');
        textLine.setAttribute('font-size', '10px');
        textLine.setAttribute('fill', 'darkgreen');
        textLine.innerHTML = text;
        return textLine;
    }
}


var lines = [];
lines.addLine = function (line) {
    this.push(line);
    return line;
}

var canvas, hourCanvas;

function DrawTimeLine(width, height, id) {

//    console.log("DrawTimeLine(width)", width);

    canvas = SVG.createCanvas(width, height, id);
    var lineElement, i, textElement, boxElement;
    var x1 = 1;
    var tx = 2;
    var numHours = 10;
    var hourSpan = width / numHours;
    //var width = GetWidth('container');

    /* Background */
    boxElement = lines.addLine(SVG.createBox(0, 0, width, 10, 'rgb(225,255,225)'));
    canvas.appendChild(boxElement);

    /* Hour indicator line */
    var schemaT = document.querySelector('.schemaTbl'); //.offsetHeight;
    var container = document.getElementById('container');
    var d = new Date();
    var currentTimeDecimal = d.getHours() + Math.round(d.getMinutes() * 100 / 60) * 0.01;
    var hourHeight = schemaT.getBoundingClientRect().bottom - container.getBoundingClientRect().bottom;
    hourCanvas = SVG.createCanvas(4, hourHeight, "hourLine");
    var line = lines.addLine(SVG.createLine(0, 0, 0, hourHeight, 'rgba(0,0,50,0.3)', 4));
    var hourLineContainer = document.getElementById('hourLineContainer');
    //alert(schemaT.getBoundingClientRect().bottom);
    //cont.setAttribute('position', 'absolute');
    hourCanvas.appendChild(line);
    //console.log(container.getBoundingClientRect().height);
    //hourCanvas.setAttribute('top', container.getBoundingClientRect().height + "px");
    //cont.style.left = "250px"; //hour * hourSpan;
    hourLineContainer.style.left = (((currentTimeDecimal - 8) * hourSpan) + container.getBoundingClientRect().left - schemaT.getBoundingClientRect().left) + "px";
//    console.log("hourLineContainer.getBoundingClientRect.top", hourLineContainer.getBoundingClientRect().top);
//    hourLineContainer.style.top = container.style.bottom + "px";
    hourLineContainer.style.zIndex = "10";
    //alert(xL.getBoundingClientRect().top);
    hourLineContainer.setAttribute('margin-top', 0 + "px");

    for (var i = 0; i <= numHours; i++) {
        if (i == numHours) {
            x1--; // Last marker
        }
        lineElement = lines.addLine(SVG.createLine(x1, 9, x1, 5, 'rgb(0,0,0)', 1));
        canvas.appendChild(lineElement);
        x1 += hourSpan;

        textElement = lines.addLine(SVG.createText(tx + 2, 8, i + 8));
        canvas.appendChild(textElement);
        tx += hourSpan;
    }

    canvas.appendChild(lines.addLine(SVG.createLine(0, height, width, height, 'rgb(0,0,0)', 1)));
}



function GetTimePosition(className, idOfTimeLine) {
    var e = document.getElementsByClassName(className);
    //var e = document.querySelector("#" + className);

    for (var i = 0; i < e.length; i++) {
        var numHours = 10;
        var width = GetWidth(idOfTimeLine)
        var hourSpan = width / numHours;
        var left = (e[i].dataset.start.replace(",", ".") - 8) * hourSpan;
        var right = width - ((e[i].dataset.end.replace(",", ".") - 8) * hourSpan);
        
        e[i].style.position = "absolute";
        e[i].style.left = Math.round(left) + "px";
        e[i].style.right = Math.round(right) + "px";
        e[i].parentNode.setAttribute('height', e[i].offsetHeight + 1 + "px");
    }
}

/*
// callback executed when canvas was found
function handleCanvas(canvas) { drawEVERYTHING(); }
function handleCanvas(hourCanvas) { drawEVERYTHING(); }

// set up the mutation observer
var observer = new MutationObserver(function (mutations, me) {
    // `mutations` is an array of mutations that occurred
    // `me` is the MutationObserver instance
    var canvas = document.getElementById('my-canvas');
    if (canvas) {
        handleCanvas(canvas);
        me.disconnect(); // stop observing
        return;
    }
});

// start observing
observer.observe(document, {
    childList: true,
    subtree: true
});
*/

//document.addEventListener("load", UpdateTimeLine);


(function () {
    //document.addEventListener('DOMContentLoaded', function () {
    //window.addEventListener("resize", DrawTimeLine(GetWidth('lala'), 10, 'timeLine'));
    //document.getElementById('container').addEventListener('resize', function () {
    //    DrawTimeLine(GetWidth('container'), 10, 'timeLine');
    //    GetTimePosition('module', 'container');
    //    GetTimePosition('lunch', 'container');
    //}, false);

    window.onresize = UpdateTimeLine();
    //window.onload = function () {
    //    // for (i = 0; i < 10; i++)  
    //    DrawTimeLine();
    //};

//    window.addEventListener("load", UpdateTimeLine);

        function UpdateTimeLine() {
        
//            let maxWidth = $('#spacer').outerWidth();
//            $('#tbl').outerWidth(maxWidth);

            let containerWidth = $("#svg-container").innerWidth();
//            console.log("containerWidth", containerWidth);
            DrawTimeLine(containerWidth, 10, 'timeLine');

            //    window.addEventListener("resize", DrawTimeLine(GetWidth('container'), 10, 'timeLine'));
            //    //document.getElementsByTagName("BODY")[0].onresize = function () { DrawTimeLine(GetWidth('container'), 10, 'timeLine'); };
            GetTimePosition('module', 'container');
            GetTimePosition('lunch', 'container');
        
    };
})();



/// Fixa till fasta svg element, ej dynamiska! Då bör sizen bli rätt!