
// Jquery Ready
$(function () {
    showConsoleLogWhenReady();
    pageInit();
});
//$(document).ready(function () {
//    showWhenReady();
//});

function pageInit() {
    evtBind();   // 事件 綁定
    paramInit(); // 參數 初始化

}

let globalParam_Null = null;
let globalParam_Empty = "";
const globalConst = "Fixed"  // 不可變
const globalObjorArray = {};  // 可變，物件或陣列

function paramInit() {
    globalParam_Null = globalParam_Null ?? "A";         // globalParam 等於 undefined or null  就會 set A
    console.log(globalParam_Null);
    globalParam_Empty = globalParam_Empty ?? "A";  // globalParam 等於 undefined or null  就會 set A
    console.log(globalParam_Empty);

    ChangeInitSelectOption("Area");
}

function evtBind() {

    $(document).on('change', '#Area', function (event, IsFirstChange) {
        let areaSelected = $(this).val();
        console.log(IsFirstChange);

        // LoadDivList(areaSelected);
        // LoadDivList_Async(areaSelected)
        LoadDivList_Promise(areaSelected)
            .then((result1) => {
                UpdateDivOptions(result1)
                if (IsFirstChange) {
                    changeSelectToFirstOption('Div')
                }
            }).catch((error) => {
                console.error(error)
            })

    })

}


function showConsoleLogWhenReady() {
    console.log("Jquery Ready")
}

function LoadDivList(d) {

    $.ajax({
        type: `Post`,
        url: `https://localhost:5000/ajax/GetDivList`,
        data: { AreaCode: d },
        dataType: `json`,
        success: function (result) {
            console.log(result)
            if (result.status === `Success`) {
                console.log("Success")
                console.log(result.data);
            } else if (result.status === `Error`) {
                console.log("Error")
                console.log(result.message);
            }
        },
        error: function (xhr, status, error) {
            console.error(error)
        }
    });

}

function LoadDivList_Promise(d) {
    return new Promise((resolve, reject) => {

        $.ajax({
            type: `Post`,
            url: `https://localhost:5000/ajax/GetDivList`,
            data: { AreaCode: d },
            dataType: `json`,
            success: function (result) {
                console.log(result)
                if (result.status === `Success`) {
                    console.log("Success")
                    console.log(result.data);
                    resolve(result.data);

                } else if (result.status === `Error`) {
                    console.log("Error")
                    console.log(result.message);
                    resolve(result.message);
                }
            },
            error: function (xhr, status, error) {
                console.error(error)
                reject(error);
            }
        });

    })



}

function UpdateDivOptions(dataSet) {
    let selectTag = $('#Div');
    selectTag.empty();

    selectTag.append(new Option("請選擇", ""))
    dataSet.forEach(divItem => {
        selectTag.append(new Option(divItem.text, divItem.value));
    })
}

async function LoadDivList_Async(d) {
    try {
        let result = await LoadDivList_Promise(d)
        UpdateDivOptions(result);
    }
    catch (error) {
        console.error(error)
    }
}


function changeSelectToFirstOption(selectId) {
    let selectOptions = $(`#${selectId} option`);
    console.log(selectOptions)
    // let validOptions = selectOptions.filter((index, option) => $(option).val());
    // let validOptions_First = validOptions.first();
    if (selectOptions.length > 0) {
        selectOptions.each(function () {     // 一定要用 function(index, element){ }
            let optionValue = $(this).val();
            console.log(optionValue)
            if (optionValue != '' && optionValue != undefined && optionValue != null) {
                $(`#${selectId}`).val(optionValue);
                return false;  // 一定要用  return false
            }
        })
        console.log("Jquery each Loop Done")  //  return false 這邊會執行
    }
}

function ChangeInitSelectOption(areaId) {
    changeSelectToFirstOption(areaId);
    let areaSelectTag = $(`#${areaId}`);
    let areaSelected = areaSelectTag.val();
    console.log(areaSelected)
    if (areaSelected != '' && areaSelected != undefined && areaSelected != null) {
        areaSelectTag.trigger('change', true) //這邊可以加入參數
        console.log("有進來 trigger")
    }
    else {
        // $('#Div').empty();
    }
}




// JS IIEF
(
    function () {
        console.log("IIEF basic format")
    }
        ()
);
// JS IIEF Example 1
(function (factory) {
    factory("Hello", "World");
})(function (a, b) {
    console.log(a + " " + b);
});
// JS IIEF Example 2
(function (c, d) {
    console.log(c + " " + d);
})("Goodbye", "Earth");




// Function vs Const
function greet_Function(name) {
    console.log(name);
}
const greet_Const = function (name) {
    console.log(name);
}
// 提升（Hoisting）：function greet(name) 是函數聲明，它會被提升到作用域的頂部，可以在定義之前調用。
//                                而 const greet = function (name) 則是函數表達式，它不會被提升，必須在定義後調用。
// 不可重新賦值：const greet 讓函數變數不能被重新賦值。如果你想確保函數的引用不被修改，const 是一個更好的選擇。
// 表達式的靈活性：const greet = function (name) 可以讓你在需要時將函數作為變數或參數傳遞，這使得你在處理動態或匿名函數時更為靈活。




// validation
// 假值 （Falsy）：JavaScript 會將一些值自動轉換為 false，例如：空字串 ""、null、undefined 和 0 和 bool false。
let inputValue = $('#inputColumn').val();
console.log(inputValue);
if (inputValue == '' || inputValue == undefined || inputValue == null) {
    console.log(" return false");
}
if (!inputValue) {    // 但可能誤判 0 ,  bool fasle
    console.log(" return false");
}
// 當使用 inputValue == null 時，它會同時檢查 null 和 undefined，因為 JavaScript 中的 == 會進行類型轉換，因此 null == undefined 會返回 true。
if (inputValue == null || inputValue === '') {
    console.log(" return false");
}