const $$ = document.querySelector.bind(document)
const $$$ = document.querySelectorAll.bind(document)

const tabs = $$$('.profile-tab-item')
const panes = $$$('.profile-content-item')

const tabActive = $$('.profile-tab-item.active')
const line = $$('.profile-tabs .line')

line.style.left = tabActive.offsetLeft+'px'
line.style.width = tabActive.offsetWidth+'px'

tabs.forEach((tab, i)=>{
    tab.onclick = function(){
        $$('.profile-tab-item.active').classList.remove('active')
        $$('.profile-content-item.active').classList.remove('active')
        tab.classList.add('active')
        panes[i].classList.add('active')
        
        line.style.left = this.offsetLeft+'px'
        line.style.width = this.offsetWidth+'px'
    }
})

// account information
const txtName = $$('.form-group #name')
const txtEmail = $$('.form-group #email')
const txtAddress = $$('.form-group #address')
const imgFile = $$('.form-group #img-file')
const changeInfoBtn = $$('.form-information .change-info-btn')
const changeBtnBox = $$('.form-information .change-info-btn-box')
const cancelBtn = $$('.change-info-btn-box .cancel-btn')
const overlay = $$('#overlay')
const changeInfoForm = $$('.form-information')

changeInfoBtn.addEventListener('click', ()=>{
    txtName.removeAttribute('disabled')
    txtAddress.removeAttribute('disabled')
    imgFile.removeAttribute('disabled')
    txtName.focus()
    changeInfoBtn.style.display = 'none'
    changeBtnBox.style.display = 'flex'
    overlay.style.display = 'block'
    changeInfoForm.style.zIndex = '10'
})

cancelBtn.addEventListener('click', ()=>{
    txtName.setAttribute('disabled', '')
    txtAddress.setAttribute('disabled', '')
    imgFile.setAttribute('disabled', '')
    changeInfoBtn.style.display = 'block'
    changeBtnBox.style.display = 'none'
    overlay.style.display = 'none'
})

// change password form
const changePwdForm = $$('.change-pwd-form')
const cancelChangePwd = $$('.change-pwd-form .change-pwd-cancel-btn')
const confirmPwdBtn = $$('.change-pwd-form .confirm-change-pwd-btn')
const changePwdBtn = $$('.card-body .change-pwd-btn')

changePwdBtn.addEventListener('click', ()=>{
    changePwdForm.style.top = '-7rem'
    overlay.style.display = 'block'
    changePwdForm.style.zIndex = '10'
    changeInfoForm.style.zIndex = '1'
})

cancelChangePwd.addEventListener('click', ()=>{
    changePwdForm.style.top = '-60rem'
    overlay.style.display = 'none'
})

confirmPwdBtn.addEventListener('click', ()=>{
    changePwdForm.style.top = '-60rem'
    overlay.style.display = 'none'
})

//Process img
const imageInput = document.getElementById('img-file');
const imagePreview = $$('.form-group-img img')
imageInput.addEventListener('change', function () {
    // Check if a file has been selected
    if (this.files && this.files[0]) {
        const reader = new FileReader();

        // When the file is loaded, set the image source to the selected file
        reader.onload = function (e) {
            imagePreview.src = e.target.result;
        };

        // Read the selected file as a data URL
        reader.readAsDataURL(this.files[0]);
    }
});