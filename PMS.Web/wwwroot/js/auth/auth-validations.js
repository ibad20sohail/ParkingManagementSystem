// Login form logic
const username = document.getElementById('Username');
const password = document.getElementById('Password');

const usernameErr = document.getElementById('usernameError');
const passwordErr = document.getElementById('passwordError');

function validateUsername() {
    const isUserNameValid = username.value.trim().length >= 3;

    username.classList.toggle('is-invalid', !isUserNameValid);
    username.classList.toggle('is-valid', isUserNameValid);

    usernameErr.textContent = isUserNameValid
        ? ""
        : "Username must be at least 3 characters.";

    return isUserValid;
}

function validatePassword() {
    const p = password.value;

    const isPasswordLengthValid = p.length >= 6;

    if (!isPasswordLengthValid) {
        password.classList.remove('is-valid');
        password.classList.add('is-invalid');

        passwordErr.textContent = "Password must be greater than 5 characters.";

        return false;
    }

    const isPasswordCharacterValid =
        /[A-Z]/.test(p) &&
        /[0-9]/.test(p) &&
        /[!@#$%^&*()]/.test(p);

    if (!isPasswordCharacterValid) {
        password.classList.remove('is-valid');
        password.classList.add('is-invalid');

        passwordErr.textContent =
            "Password must contain at least 1 uppercase letter, 1 number, and 1 special character.";

        return false;
    }

    password.classList.remove('is-invalid');
    password.classList.add('is-valid');

    passwordErr.textContent = "";

    return true;
}

function validateLoginForm() {
    const isUsernameValid = validateUsername();
    const isPasswordValid = validatePassword();

    return isUsernameValid && isPasswordValid;
}

user.addEventListener('input', validateUser);
pass.addEventListener('input', validatePassword);

const loginForm = document.getElementById('loginForm');

if (loginForm) {
    loginForm.addEventListener('submit', function (event) {
        if (!validateLoginForm()) {
            event.preventDefault();
        }
    });
}