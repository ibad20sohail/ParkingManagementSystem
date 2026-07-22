// Login form logic
const user = document.getElementById('UserName');
const pass = document.getElementById('Password');

const userErr = document.getElementById('usernameError');
const passErr = document.getElementById('passwordError');

function validateUser() {
    const isUserValid = user.value.trim().length >= 3;
    user.classList.toggle('is-invalid', !isUserValid);
    user.classList.toggle('is-valid', isUserValid);
    userErr.textContent = isUserValid ? "" : "Username must be at least 3 characters.";
    return isUserValid; 
}
function validatePassword() {
    const p = pass.value;
    const isPassValid = p.length >= 6 && /[A-Z]/.test(p) && /[0-9]/.test(p) && /[!@#$%^&*()]/.test(p);

    pass.classList.toggle('is-invalid', !isPassValid);
    pass.classList.toggle('is-valid', isPassValid);
    passErr.textContent = isPassValid ? "" : "Password must be greater than 5 characters with 1 capital, 1 number, and 1 special char.";
    return isPassValid;
}
function validateLoginForm() {
    const isUserValid = validateUser();
    const isPasswordValid = validatePassword();

    return isUserValid && isPasswordValid;
}

user.addEventListener('input', validateUser);
pass.addEventListener('input', validatePassword);

const loginForm = document.getElementById('loginForm');
if (loginForm) {
    loginForm.addEventListener('submit', function (event) {
        const isFormValid = validateLoginForm();

        if (!isFormValid) {
            event.preventDefault();
        }
    });
}
