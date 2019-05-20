var json;
var baseInner = null;
var baseContent = null;
var current = null;

function update() {
    let xhr = new XMLHttpRequest();
    xhr.addEventListener("readystatechange", function () {
        if (this.readyState === 4) {
            if (this.status == 200) {
                if (baseInner === null) {
                    baseInner = document.getElementById('tableDetails').innerHTML;
                }
                if (baseContent === null) {
                    baseContent = document.getElementById('tableContent').innerHTML;
                }
                let finalHtml = baseContent;
                json = JSON.parse(this.responseText);
                let dict = {};
                let consom = {};
                json.allDevices.forEach(function(elem) {
                    if (!(elem.roomName in dict)) {
                        dict[elem.roomName] = elem.isOn;
                        consom[elem.roomName] = elem.consumption[0].value;
                    }
                    else {
                        if (elem.isOn) {
                            dict[elem.roomName] = true;
                        }
                        consom[elem.roomName] += elem.consumption[0].value;
                    }
                });
                for (var key in dict) {
                    finalHtml += '<tr id="contentLine"><td id="left"><nav>' + key + '</nav></td><td><nav>' + (dict[key] ? "Active" : "Inactive")
                        + '</nav></td><td><nav>' + consom[key] / 1000
                        + ' kWh</nav></td><td><nav>0</nav></td><td id="right"><nav>0</nav></td><td><button class="button" onclick="getDetails(\'' + key + '\')">More details</button></td></tr>';
                }
                document.getElementById("tableContent").innerHTML = finalHtml;

                if (current !== null) {
                    getDetails(current);
                }

                google.charts.load('current', {'packages':['corechart']});
                google.charts.setOnLoadCallback(drawGraphs);
            } else {
                window.location.replace("http://hec.zirk.eu/");
            }
        }
    });
    xhr.open("POST", "http://93.118.34.39:5151/devices");
    xhr.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
    xhr.send("token=" + sessionStorage['token']);
}

update();
window.setInterval(update, 60000); // 1 minute

function getDetails(roomName) {
    current = roomName;
    let finalHtml = "";
    json.allDevices.forEach(function(elem) {
        if (elem.roomName == roomName) {
            tomorrowVal = elem.consumption[1]; // If it's the first day launching the app, will be undefined
            finalHtml += '<tr id="contentLine"><td id="left"><nav>' + elem.type + '</nav></td><td><nav>' + elem.name
                + '</nav></td><td><nav>' + (elem.isOn ? "Active" : "Inactive")
                + '</nav></td><td><nav>' + elem.consumption[0].value / 1000
                + ' kWh</nav></td><td id="right"><nav>' + (typeof lastname === "undefined" ? 0 : elem.consumption[1].value / 1000)
                + ' kWh</nav></td><td><button class="button" onclick="switchState(\'' + elem.id + '\', \'' + !elem.isOn + '\')">Action</button></td></tr>';
        }
    });
    document.getElementById("tableDetails").innerHTML = baseInner + finalHtml;
    window.scrollTo(0, document.body.scrollHeight);
}

function switchState(id, nextState) {
    let xhr = new XMLHttpRequest();
    xhr.addEventListener("readystatechange", function () {
        if (this.readyState === 4) {
            if (this.status == 204) {
                update();
            } else {
                window.location.replace("http://hec.zirk.eu/");
            }
        }
    });
    xhr.open("POST", "http://93.118.34.39:5151/switch");
    xhr.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
    xhr.send("token=" + sessionStorage['token'] + "&id=" + id + "&status=" + nextState);
}

function drawGraphs() {
    let options = {
        title: ''
    };

    let finalData = [["Type", "Value"]];
    let categories = [];
    json.allDevices.forEach(function(elem) {
        if (!categories.includes(elem.type)) {
            categories.push(elem.type);
        }
    });
    categories.forEach(function(elem) {
        let value = 0;
        json.allDevices.forEach(function(elem2) {
            if (elem == elem2.type) {
                value += elem2.consumption[0].value;
            }
        });
        finalData.push([elem, value]);
    });

    let data = google.visualization.arrayToDataTable(finalData);

    options.title = 'Breakdown of energy consumption';
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