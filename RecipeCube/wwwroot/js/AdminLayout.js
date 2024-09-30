
    $(document).ready(function () {
    let controller = '';
    let parentId = '';
    let filterRow = null;
    // 定義一個全局的 orderId 變數
    let orderId = null;
    let orderpass = false;
    // 定義保存的頁面與搜尋條件
    let savedPage = null;
    let savedSearchQuery = null;

            // 初始化Sidebar卷軸
            if (navigator.userAgent.indexOf('Win') > -1 && document.querySelector('#sidenav-scrollbar')) {
        Scrollbar.init(document.querySelector('#sidenav-scrollbar'), { damping: '0.5' });
            }

    function initializePerfectScrollbar(selector) {
        new PerfectScrollbar(selector, { damping: 0.5 });
            }

    function initializeDataTableWithFilter(columnIndex) {
                //console.log("columnIndex:", columnIndex)
                var table = $('.table').DataTable({

        language: {
        url: "//cdn.datatables.net/plug-ins/2.1.6/i18n/zh-HANT.json",
    paginate: {first: "«", previous: "‹", next: "›", last: "»" },
                    },
    initComplete: function () {
                        if (columnIndex !== null) {
                            var column = this.api().column(columnIndex);
    let label = document.createElement('label');
    label.innerHTML = '篩選:';
    label.classList.add('ms-5');
    let select = document.createElement('select');
    select.classList.add('form-select', 'form-select-sm', 'ms-1', 'w-50');
    select.add(new Option(''));
    select.addEventListener('change', function () {
        column.search(this.value).draw();
                            });
    $("div.dt-length").css('white-space', 'nowrap');
    $("div.dt-length label").after(label, select);
    column.data().unique().sort().each(function (d) {
        select.add(new Option(d));
                            });
                        }
                    }
                });
            }


            // 載入Partial 原版
            // function loadPartial() {
        //     $.ajax({
        //         url: `/Admin/${controller}/${parentId}IndexPartial`,
        //         type: 'GET',
        //         success: function (data) {
        //             $('#dynamic-content').html(data);
        //             initializeDataTableWithFilter(filterRow);
        //             // 根據每個局部檢視返回的標題設置頁面標題
        //             var newTitle = $('#dynamic-content').find('h1').text();
        //             //console.log(newTitle);
        //             document.title = newTitle + ' - 食譜魔方';  // 設置頁面標題
        //             //console.log(document.title);
        //             // 更新麵包屑
        //             $('ol.breadcrumb .breadcrumb-item.active').text(newTitle);
        //             $('h6.font-weight-bolder').text(newTitle);
        //         },
        //         error: function (xhr, status, error) {
        //             alert("Error: " + error);
        //         }
        //     });
        // }

        // 初始化 DataTable 並加載資料
        async function firstloadPartial() {
            try {
                var response = await fetch(`/Admin/${controller}/${parentId}IndexPartial`, {
                    method: "GET",
                });
                if (response.ok) {
                    var data = await response.text();
                    $('#dynamic-content').html(data);
                    initializeDataTableWithFilter(filterRow);
                    // 設定頁面標題
                    let newTitle = $('#dynamic-content').find('h1').text();
                    document.title = newTitle + ' - 食譜魔方';
                    $('ol.breadcrumb .breadcrumb-item.active').text(newTitle);
                    $('h6.font-weight-bolder').text(newTitle);
                }
                else {
                    throw new Error(`HTTP error! Status: ${response.status}`);
                }
            } catch (error) {
                console.error("Error loading partial content:", error);
            }
        }

            async function reloadPartial() {
                // 檢查 DataTable 是否已經初始化並儲存當前頁面資訊
                if ($.fn.DataTable && $.fn.DataTable.isDataTable('.table')) {
        let table = $('.table').DataTable();

    // 儲存頁面資訊
    let pageInfo = table.page.info();
    savedPage = pageInfo.page;  // 保存當前頁碼
    savedSearchQuery = table.search();  // 保存當前搜尋條件
                } else {
        console.warn('Table is not initialized yet or DataTable is not loaded.');
                }
    try {
                    var response = await fetch(`/Admin/${controller}/${parentId}IndexPartial`, {
        method: "GET",
                    });
    if (response.ok) {
                        var data = await response.text();
    $('#dynamic-content').html(data);  // 替換內容

    if ($.fn.dataTable) {
                            // console.log('Attempting to initialize DataTable...');

                            if ($('.table').length) {
        let newTable = $('.table').DataTable({
        language: {
        url: "//cdn.datatables.net/plug-ins/2.1.6/i18n/zh-HANT.json",
    paginate: {first: "«", previous: "‹", next: "›", last: "»" },
                                    },
    // 初始化完成後還原頁碼和搜尋條件
    initComplete: function () {
                                        // 還原搜尋條件
                                        if (savedSearchQuery !== null) {
        newTable.search(savedSearchQuery).draw(false);
    $('.table_filter input[type="search"]').val(savedSearchQuery);
                                            // console.log('Restored search query: ' + savedSearchQuery);
                                        }

                                        // 還原頁碼
                                        if (savedPage !== null && savedPage >= 0) {
        newTable.page(savedPage).draw(false);  // 還原到保存的頁碼
                                            // console.log('Restored page: ' + savedPage);
                                        }
                                    }
                                });
                            } else {
        console.warn('Table element not found for DataTable initialization!');
                            }
                        } else {
        console.error('DataTable library not loaded!');
                        }
                    } else {
                        throw new Error(`HTTP error! Status: ${response.status}`);
                    }
                } catch (error) {
        console.error("Error reloading partial content:", error);
                }
            }



    // 載入Modal
    async function loadModalData(url, modalSelector, successCallback) {
                try {
                    var response = await fetch(url, {
        method: "GET",
                    });
    if (response.ok) {
                        var data = await response.text();
    $(modalSelector + ' .modal-body').html(data);
    initializePerfectScrollbar(modalSelector + ' .modal-body');
    $(modalSelector).modal('show');
    if (successCallback) successCallback();
                    }
    else {
        alert('發生錯誤，無法載入資料');
                    }
                } catch (error) {
        console.log(error);
    alert('發生錯誤，無法載入資料');
                }
                // $.ajax({
        //     url: url,
        //     type: 'GET',
        //     success: function (data) {
        //         $(modalSelector + ' .modal-body').html(data);
        //         initializePerfectScrollbar(modalSelector + ' .modal-body');
        //         $(modalSelector).modal('show');
        //         if (successCallback) successCallback();
        //     },
        //     error: function () {
        //         alert('發生錯誤，無法載入資料');
        //     }
        // });
    }

            // 處理Form的Submit
    async function handleFormSubmit(formSelector, modalSelector) {
        $(document).on('submit', formSelector, async function (e) {
            e.preventDefault();
            // 使用 FormData 來處理文件上傳
            var formData = new FormData(this);
            try {
                var respnose = await fetch($(this).attr('action'), {
                    method: "POST",
                    body: formData,
                });
                if (response.ok) {
                    var data = await response.json();
                    $(modalSelector).modal('hide');
                    if (controller === "OrderItems" && orderId && orderpass == true) {
                        // 使用帶有 orderId 的載入方法
                        loadPartialWithOrderId(orderId);
                    } else {
                        reloadPartial();
                    }
                }
                else {
                    $(modalSelector + ' .modal-body').html(data);
                }
            } catch (error) {
                alert(error);
            }
        });
            }


    // Sidebar的反應
    $("ul.navbar-nav .nav-link").on('click', function (e) {
        $("ul.navbar-nav .nav-link").removeClass("active");
    $(this).addClass("active");
    parentId = $(this).parent().attr('id');
    if (parentId !== "Home") {
        e.preventDefault();
                }
    $('#dynamic-content').html('<div>Loading...</div>');

    switch (parentId) {
                    case 'User': controller = 'Users'; filterRow = null; break;
    case 'Group': controller = 'UserGroups'; filterRow = 0; break;
    case 'PreferedFood': controller = 'PreferedIngredients'; filterRow = 0; break;
    case 'ExclusiveFood': controller = 'ExclusiveIngredients'; filterRow = 0; break;
    case 'Inventory': controller = 'Inventories'; filterRow = 0; break;
    case 'Pantry': controller = 'PantryManagements'; filterRow = 0; break;
    case 'Ingredient': controller = 'Ingredients'; filterRow = 1; break;
    case 'RecipeIngredient': controller = 'RecipeIngredients'; filterRow = 3; break;
    case 'Recipe': controller = 'Recipes'; filterRow = 2; break;
    case 'Product': controller = 'Products'; filterRow = 0; break;
    case 'Order': controller = 'Orders'; filterRow = 0; break;
    case 'OrderItem': controller = 'OrderItems'; filterRow = 0; orderpass = false; break;
    default: console.error('Controller 未定義或 parentId 為 Home'); return;
                }
    firstloadPartial();
            });

    $(document).on('click', '.edit', function () {
        let id = $(this).data('id');
    loadModalData(`/Admin/${controller}/EditPartial/` + id, '#editModal');
            });

    $(document).on('click', '.detail', function () {
        let id = $(this).data('id');
    loadModalData(`/Admin/${controller}/DetailsPartial/` + id, '#detailsModal');
            });

    $(document).on('click', '.create', function () {
        loadModalData(`/Admin/${controller}/CreatePartial`, '#createModal');
            });

    $(document).on('click', '.delete', function () {
        let id = $(this).data('id');
    loadModalData(`/Admin/${controller}/DeletePartial/` + id, '#deleteModal');
            });

    $(document).on('click', '#openeditModal', function () {
        let id = $(this).data('id');
    $('#detailsModal').modal('hide');
    loadModalData(`/Admin/${controller}/EditPartial/` + id, '#editModal');
            });

    // Form的submit
    handleFormSubmit('#editForm', '#editModal');
    handleFormSubmit('#createForm', '#createModal');
    handleFormSubmit('#deleteForm', '#deleteModal');

    // Modal關閉時的反應
    $('.modal').on('hidden.bs.modal', function () {
        $(this).find('.modal-body').html('');  // 清空 modal 內容
    reloadPartial();
            });

    //給訂單跳轉到訂單詳細時用的Code
    $(document).on('click', '.detailOrder', async function (e) {
        e.preventDefault();  // 阻止預設行為

    $("ul.navbar-nav .nav-link").removeClass("active");  // 清除其他項目的 active 狀態
    $("#OrderItem").children(".nav-link").addClass("active");  // 設定訂單明細項目為 active

    orderId = $(this).data('id');  // 獲取按鈕的 data-id (即 orderId)
    orderpass = true;
    controller = 'OrderItems';  // 切換 controller 為 'OrderItems'
    parentId = 'OrderItem';
    filterRow = 0;  // 設置篩選的列索引，這裡假設 orderId 是第一列 (索引 0)

    // 更新動態內容
    $('#dynamic-content').html('<div>Loading...</div>');

    try {
                    // 使用 fetch 來替代 $.ajax
                    const response = await fetch('/Admin/OrderItems/OrderItemIndexPartial', {
        method: 'POST',
    headers: {
        'Content-Type': 'application/json'
                        },
    body: JSON.stringify({orderid: orderId })  // 傳遞 orderId 作為參數
                    });

    if (response.ok) {
                        const data = await response.text();  // 獲取返回的 HTML 內容
    $('#dynamic-content').html(data);  // 更新 DOM 為返回的部分檢視

    // 初始化 DataTable 並直接篩選
    const table = new DataTable("table", {
        initComplete: function () {
        // 自動篩選與 orderId 相關的訂單
        initializeFilters(this, orderId);  // 傳遞 orderId 作為預設篩選值
                            },
    language: {
        url: "//cdn.datatables.net/plug-ins/2.1.6/i18n/zh-HANT.json",
    paginate: {first: "«", previous: "‹", next: "›", last: "»" },
                            },
                        });
                    } else {
                        throw new Error(`Error: ${response.statusText}`);
                    }
                } catch (error) {
        alert(`Error: ${error}`);
                }

    // 使用自定義的 loadPartialWithOrderId 來處理包含 orderId 的部分檢視跳轉
    loadPartialWithOrderId(orderId);
            });


    // 初始化篩選選項
    function initializeFilters(dtInstance, orderId) {
                var columnIndex = 0;  // 假設 orderId 在第一列 (index = 0)
    var column = dtInstance.api().column(columnIndex);
    let label = document.createElement('label');
    label.innerHTML = '篩選:';
    label.classList.add('ms-5');
    let select = document.createElement('select');
    select.classList.add('form-select', 'form-select-sm', 'ms-1', 'w-50');
    select.add(new Option('')); // 添加空選項

    // 添加可選項
    column.data().unique().sort().each(function (d) {
        select.add(new Option(d));
                });

    // 設置預設值為 orderId 並自動觸發篩選
    select.value = orderId;  // 將選擇框的值設為 orderId
    select.addEventListener('change', function () {
        column.search(this.value).draw();  // 當選擇改變時，進行篩選
                });

    // 自動觸發篩選
    column.search(orderId).draw();  // 直接觸發篩選
    $("div.dt-length").css('white-space', 'nowrap');
    $("div.dt-length label").after(label, select);
            }
    // 給特定情況的 loadPartial 函數，處理包含 orderId 的跳轉
    async function loadPartialWithOrderId(orderId) {
                try {
                    // 使用 fetch 來替代 $.ajax
                    const response = await fetch(`/Admin/${controller}/${parentId}IndexPartial?orderid=${orderId}`, {
        method: 'GET'
                    });

    if (response.ok) {
                        const data = await response.text();  // 獲取返回的 HTML 內容
    $('#dynamic-content').html(data);  // 更新 DOM

    // 初始化 DataTable 並應用篩選條件
    const table = new DataTable("table", {
        initComplete: function () {
        initializeFilters(this, orderId);  // 將 orderId 作為篩選條件
                            },
    language: {
        url: "//cdn.datatables.net/plug-ins/2.1.6/i18n/zh-HANT.json",
    paginate: {first: "«", previous: "‹", next: "›", last: "»" },
                            },
                        });
                    } else {
                        throw new Error(`Error: ${response.statusText}`);
                    }
                } catch (error) {
        alert(`Error: ${error}`);
                }
            }
        });
