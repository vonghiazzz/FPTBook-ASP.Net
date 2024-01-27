const $$ = document.querySelector.bind(document)
const $$$ = document.querySelectorAll.bind(document)

const listPrice = $$$('.cart-list .item-price')
const qtyList = $$$('.cart-list .item-qty input')
const total = $$('.checkout-action b')
const checkoutBtn = $$('.checkout-action a')

let sum
function calculateCart(){
    sum = 0
    listPrice.forEach((price, i)=>{
        sum+= price.innerHTML*qtyList[i].value
    })
    total.innerHTML = sum.toFixed(2)
}

calculateCart()

qtyList.forEach(input=>{
    input.addEventListener('input', ()=>{
        if(input.value>=1&&input.value<=9999){
            calculateCart()
        }
    })
})

function validateCart() {
    let isValid = true
    qtyList.forEach(input=>{
        if(input.value<1 || input.value>9999){
            isValid = false
        }
    })
    
    return isValid;
}

function checkQty(event){
    if(!(validateCart())){
        alert('Invalid product quantity!!!')
        event.preventDefault()
    }
}

checkoutBtn.addEventListener('click', checkQty)
