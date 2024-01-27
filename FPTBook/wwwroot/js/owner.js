const $$ = document.querySelector.bind(document)
const $$$ = document.querySelectorAll.bind(document)

let thisPage = 1;
let limit = 6;
let list = $$$('.list-book .book-item');

function loadItem(){
    let beginGet = limit * (thisPage - 1);
    let endGet = limit * thisPage - 1;
    list.forEach((item, key)=>{
        if(key >= beginGet && key <= endGet){
            item.style.display = 'block';
        }else{
            item.style.display = 'none';
        }
    })
    listPage();
}
loadItem();
function listPage(){
    let count = Math.ceil(list.length / limit);
    document.querySelector('.book-management-display .list-page').innerHTML = '';

    if(thisPage != 1){
        let prev = document.createElement('li');
        prev.innerText = 'PREV';
        prev.setAttribute('onclick', "changePage(" + (thisPage - 1) + ")");
        document.querySelector('.list-page').appendChild(prev);
    }

    for(i = 1; i <= count; i++){
        let newPage = document.createElement('li');
        newPage.innerText = i;
        if(i == thisPage){
            newPage.classList.add('active');
        }
        newPage.setAttribute('onclick', "changePage(" + i + ")");
        document.querySelector('.list-page').appendChild(newPage);
    }

    if(thisPage != count){
        let next = document.createElement('li');
        next.innerText = 'NEXT';
        next.setAttribute('onclick', "changePage(" + (thisPage + 1) + ")");
        document.querySelector('.list-page').appendChild(next);
    }
}

function changePage(i){
    thisPage = i;
    loadItem();
}

// process img file
const imageInputAuthor = document.getElementById('file-author');
const imagePreviewAuthor = document.getElementById('author-preview');

const imageInputProduct = document.getElementById('file-book');
const imagePreviewProduct = document.getElementById('product-preview');

// Add an event listener to the input element
imageInputAuthor.addEventListener('change', function () {
    // Check if a file has been selected
    if (this.files && this.files[0]) {
        const reader = new FileReader();

        // When the file is loaded, set the image source to the selected file
        reader.onload = function (e) {
            imagePreviewAuthor.src = e.target.result;
        };

        // Read the selected file as a data URL
        reader.readAsDataURL(this.files[0]);
    }
});

imageInputProduct.addEventListener('change', function () {
    // Check if a file has been selected
    if (this.files && this.files[0]) {
        const reader = new FileReader();

        // When the file is loaded, set the image source to the selected file
        reader.onload = function (e) {
            imagePreviewProduct.src = e.target.result;
        };

        // Read the selected file as a data URL
        reader.readAsDataURL(this.files[0]);
    }
});

// process active
const processList = $$$('.process-item')
const formList = $$$('.form-create')

processList.forEach((item, index)=>{
    item.onclick = function(){
        $$('.process-item.active').classList.remove('active')
        $$('.form-create.active').classList.remove('active')

        item.classList.add('active')
        formList[index].classList.add('active')
    }
})

// add book button
$$('.add-book-btn').onclick = function(){
    $$('.book-management-add').style.display = 'block'
    $$('.book-management-display').style.display = 'none'
}

// back button add book
const backBtn = $$('.add-book-back-btn')

backBtn.onclick = function(){
    $$('.book-management-add').style.display = 'none'
    $$('.book-management-display').style.display = 'block'
}

// order record section
const detailList = $$$('.table td a')
detailList.forEach((item)=>{
    item.onclick = function () {
        $$('.order-display').style.display = 'none'
        $$('.order-detail').style.display = 'block'
    }
})

// nav
const sectionList = $$$('.owner-content .item-section')
$$$('.owner-content .side-nav .nav-item').forEach((item, index) =>{
    item.onclick = function(){
        $$('.item-section.active').classList.remove('active')
        sectionList[index].classList.add('active')
    }
})

// request
$$('.view-request-history').onclick = function () {
    $$('.table-request').style.display = 'table'
    $$('#form-request-genre').style.display = 'none'
    this.style.display = 'none'
    $$('.back-request').style.display = 'block'
}

$$('.back-request').onclick = function () {
    $$('.table-request').style.display = 'none'
    $$('#form-request-genre').style.display = 'flex'
    this.style.display = 'none'
    $$('.view-request-history').style.display = 'block'
}