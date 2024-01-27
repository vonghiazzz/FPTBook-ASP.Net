const $$ = document.querySelector.bind(document)
const $$$ = document.querySelectorAll.bind(document)


const statusBook = $$('.available')
const addCartBtn = $$('.add-cart')
const addWishlist = $$('.add-wishlist')

// check status of book
if (statusBook.style.display == 'none') {
    addCartBtn.onclick = function (e) {
        e.preventDefault()
    }
    addWishlist.onclick = function (e) {
        e.preventDefault()
    }
}

// More detail
const tabs = $$$('.tab-item')
const panes = $$$('.tab-pane')

const tabActive = $$('.tab-item.active')
const line = $$('.tabs .line')

line.style.left = tabActive.offsetLeft+'px'
line.style.width = tabActive.offsetWidth+'px'

tabs.forEach((tab, i)=>{
    tab.onclick = function(){
        $$('.tab-item.active').classList.remove('active')
        $$('.tab-pane.active').classList.remove('active')
        tab.classList.add('active')
        panes[i].classList.add('active')
        
        line.style.left = this.offsetLeft+'px'
        line.style.width = this.offsetWidth+'px'
    }
})

// related book
const listContainNew = $$('.product-list-new')
const listContainBest = $$('.product-list-best')

const previousNew = $$('.section-new .prev')
const nextNew = $$('.section-new .next')

const previousBest = $$('.section-best-seller .prev')
const nextBest = $$('.section-best-seller .next')

previousNew.addEventListener('click', () => {
    listContainNew.scrollBy({
        left: -100,
        behavior: 'smooth',
    });
});

nextNew.addEventListener('click', () => {
    listContainNew.scrollBy({
        left: 100,
        behavior: 'smooth',
    });
});