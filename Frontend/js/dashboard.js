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
                let oldConsom = {};
                json.allDevices.forEach(function(elem) {
                    if (!(elem.roomName in dict)) {
                        dict[elem.roomName] = elem.isOn;
                        consom[elem.roomName] = elem.consumption[elem.consumption.length - 1].value;
                        if (elem.consumption.length > 1)
                            oldConsom[elem.roomName] = elem.consumption[elem.consumption.length - 2].value;
                        else
                            oldConsom[elem.roomName] = 0;
                    }
                    else {
                        if (elem.isOn) {
                            dict[elem.roomName] = true;
                        }
                        consom[elem.roomName] += elem.consumption[elem.consumption.length - 1].value;
                        if (elem.consumption.length > 1)
                            oldConsom[elem.roomName] += elem.consumption[elem.consumption.length - 2].value;
                    }
                });
                for (var key in dict) {
                    finalHtml += '<tr id="contentLine"><td id="left"><nav>' + key + '</nav></td><td><nav id="' + (dict[key] ? 'green">Active' : 'red">Inactive')
                        + '</nav></td><td><nav>' + consom[key] / 1000
                        + ' kWh</nav></td><td><nav>' + oldConsom[key] / 1000
                        + ' kWh</nav></td><td id="right"><nav>' + (oldConsom[key] === 0 ? 0 : Number(consom[key] / oldConsom[key] * consom[key] / 1000).toFixed(3))
                        + ' kWh</nav></td><td><button class="button" onclick="getDetails(\'' + key + '\')">More details</button></td></tr>';
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
            finalHtml += '<tr id="contentLine"><td id="left"><nav>' + elem.type + '</nav></td><td><nav>' + elem.name
                + '</nav></td><td><nav id="' + (elem.isOn ? 'green">Active' : 'red">Inactive')
                + '</nav></td><td><nav>' + elem.consumption[elem.consumption.length - 1].value / 1000
                + ' kWh</nav></td><td id="right"><nav>' + (elem.consumption.length == 1 ? 0 : elem.consumption[elem.consumption.length - 2].value / 1000)
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
                value += elem2.consumption[elem2.consumption.length - 1].value;
            }
        });
        finalData.push([elem, value / 1000]);
    });

    let data = google.visualization.arrayToDataTable(finalData);

    options.title = 'Breakdown of energy consumption (kWh)';
    let chart = new google.visualization.PieChart(document.getElementById('consumptionBreakdownGraph'));
    chart.draw(data, options);

    finalData = [["Type", "Energy"]];
    categories = [];
    json.allDevices.forEach(function(elem) {
        if (!categories.includes(elem.roomName)) {
            categories.push(elem.roomName);
        }
    });
    categories.forEach(function(elem) {
        let value = 0;
        json.allDevices.forEach(function(elem2) {
            if (elem == elem2.roomName) {
                value += elem2.consumption[elem2.consumption.length - 1].value;
            }
        });
        finalData.push([elem, value / 1000]);
    });
    data = google.visualization.arrayToDataTable(finalData);

    options.title = 'Energy consumption per room (kWh)';
    chart = new google.visualization.ColumnChart(document.getElementById('comsumptionPerBuildingsGraph'));
    chart.draw(data, options);

    finalData = [["Type", "Energy"]];
    for (let i = 10; i >= 1; i--) {
        let value = 0;
        json.allDevices.forEach(function(elem) {
            if (elem.consumption.length >= i) {
                value += elem.consumption[elem.consumption.length - i].value;
            }
        });
        finalData.push(["D - " + (i - 1), value / 1000]);
    }

    data = google.visualization.arrayToDataTable(finalData);

    options.title = 'Energy consumption per day (kWh)';
    chart = new google.visualization.LineChart(document.getElementById('comsumptionPerHoursGraph'));
    chart.draw(data, options);
}