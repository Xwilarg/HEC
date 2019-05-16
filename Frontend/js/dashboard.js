var json;
var dict = {};
var baseInner;

let xhr = new XMLHttpRequest();
xhr.addEventListener("readystatechange", function () {
    if (this.readyState === 4) {
        if (this.status == 200) {
            baseInner = document.getElementById('tableDetails').innerHTML;
            let finalHtml = "";
            json = JSON.parse(this.responseText);
            json.allDevices.forEach(function(elem) {
                if (!(elem.roomName in dict)) {
                    dict[elem.roomName] = elem.isOn;
                }
                else if (elem.isOn) {
                    dict[elem.roomName] = true;
                }
            });
            for (var key in dict) {
                finalHtml += '<tr id="contentLine"><td id="left"><nav>' + key + '</nav></td><td><nav>' + (dict[key] ? "Active" : "Inactive")
                    + '</nav></td><td><nav>0</nav></td><td><nav>0</nav></td><td id="right"><nav>0</nav></td><td><button class="button" onclick="getDetails(\'' + key + '\')">More details</button></td></tr>';
            }
            document.getElementById("tableContent").innerHTML += finalHtml;
        } else {
            window.location.replace("http://hec.zirk.eu/");
        }
    }
});
xhr.open("POST", "http://93.118.34.39:5151/devices");
xhr.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
xhr.send("token=" + sessionStorage['token']);

google.charts.load('current', {'packages':['corechart']});
google.charts.setOnLoadCallback(drawGraphs);

function getDetails(roomName) {
    let finalHtml = "";
    json.allDevices.forEach(function(elem) {
        if (elem.roomName == roomName) {
            finalHtml += '<tr id="contentLine"><td id="left"><nav>' + elem.type + '</nav></td><td><nav>' + elem.name
                + '</nav></td><td><nav>' + (elem.isOn ? "Active" : "Inactive")
                + '</nav></td><td><nav>0</nav></td><td id="right"><nav>0</nav></td><td><button class="button" onclick="">Action</button></td></tr>';
        }
    });
    document.getElementById("tableDetails").innerHTML = baseInner + finalHtml;
    window.scrollTo(0, document.body.scrollHeight);
}

function drawGraphs() {
    let options = {
        title: ''
    };

    let data = google.visualization.arrayToDataTable([
        ['Data1', 'Data2'],
        ['Element1', 45],
        ['Element2', 75],
        ['Element3', 42],
        ['Element4', 34],
        ['Element5', 8]
    ]);

    options.title = 'Sample Pie Chart';
    let chart = new google.visualization.PieChart(document.getElementById('consumptionBreakdownGraph'));
    chart.draw(data, options);

    data = google.visualization.arrayToDataTable([
        ["Data1", 'Data2'],
        ["Element1", 43],
        ["Element2", 25],
        ["Element3", 52],
        ["Element4", 12]
    ]);

    options.title = 'Sample Column Chart';
    chart = new google.visualization.ColumnChart(document.getElementById('comsumptionPerBuildingsGraph'));
    chart.draw(data, options);

    data = google.visualization.arrayToDataTable([
        ["Data1", 'Data2'],
        ["Element1", 8],
        ["Element2", 5],
        ["Element3", 12],
        ["Element4", 6],
        ["Element5", 8],
        ["Element6", 3],
        ["Element7", 15],
        ["Element8", 9],
        ["Element9", 10],
        ["Element10", 7]
    ]);

    options.title = 'Sample Line Chart';
    chart = new google.visualization.LineChart(document.getElementById('comsumptionPerHoursGraph'));
    chart.draw(data, options);
}