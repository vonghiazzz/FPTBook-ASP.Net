const $$ = document.querySelector.bind(document)
const $$$ = document.querySelectorAll.bind(document)

// customer detail
function positionDetailForm(paragraph) {
    const paragraphRect = paragraph.getBoundingClientRect();
  
    // Calculate the position of the detail form
    const top = paragraphRect.bottom + window.scrollY;
    const left = paragraphRect.left + window.scrollX;
  
    // Set the position of the detail form
    $$('.customer-detail').style.position = 'absolute';
    $$('.customer-detail').style.top = top + 'px';
    $$('.customer-detail').style.left = (left-500) + 'px';
    $$('.customer-detail').style.display = 'flex';

    $$('.owner-detail').style.position = 'absolute';
    $$('.owner-detail').style.top = top + 'px';
    $$('.owner-detail').style.left = (left-500) + 'px';
    $$('.owner-detail').style.display = 'flex';
  }

$$$('.customer-display .table .detail p').forEach(item=>{
    item.onclick = function(){
        positionDetailForm(item)
        $$('#overlay-owner').style.display = 'block'
    }
})

$$$('.owner-display .table .detail p').forEach(item=>{
    item.onclick = function(){
        positionDetailForm(item)
        $$('#overlay-owner').style.display = 'block'
    }
})

//$$('.customer-detail .back').onclick = function(){
//    $$('.customer-detail').style.display = 'none'
//    $$('#overlay-customer').style.display = 'none';
//}

//$$('.owner-detail .back').onclick = function(){
//    $$('.owner-detail').style.display = 'none'
//    $$('#overlay-owner').style.display = 'none';
//}

// genre create
$$('.genre-display .add-btn').onclick = function(){
    $$('.genre-display').style.display = 'none'
    $$('.genre-create').style.display = 'flex'
}

$$('.genre-create .back-btn').onclick = function(){
    $$('.genre-display').style.display = 'block'
    $$('.genre-create').style.display = 'none'
}

// nav
const adminItemList = $$$('.admin-content .item-section')
$$$('.admin-content .side-nav .nav-item').forEach((item, index)=>{
    item.onclick = function(){
        $$('.item-section.active').classList.remove('active')
        adminItemList[index].classList.add('active')
    }
})


$$('.action-genre .delete').onclick = function () {
    confirm("Do you want to remove the genre?");
}