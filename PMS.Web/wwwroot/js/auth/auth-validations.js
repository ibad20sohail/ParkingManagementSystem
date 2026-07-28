const loginForm = document.getElementById('loginForm');
const forgetPasswordForm = document.getElementById('forgetPasswordForm');
const resetPasswordForm = document.getElementById('resetPasswordForm');

// Input fields
const username = document.getElementById('username');
const password = document.getElementById('password');
const newPassword = document.getElementById('newPassword');
const confirmPassword = document.getElementById('confirmPassword');

// Error elements
const usernameErr = document.getElementById('usernameError');
const passwordErr = document.getElementById('passwordError');
const newPasswordErr = document.getElementById('newPasswordError');
const confirmPasswordErr = document.getElementById('confirmPasswordError');

// Form submit events
if (loginForm) {
    loginForm.addEventListener('submit', function (event) {
        if (!validateLoginForm()) {
            event.preventDefault();
        }
    });
}

if (forgetPasswordForm) {
    forgetPasswordForm.addEventListener('submit', function (event) {
        if (!validateForgotPasswordForm()) {
            event.preventDefault();
        }
    });
}

if (resetPasswordForm) {
    resetPasswordForm.addEventListener('submit', function (event) {
        if (!validateResetPasswordForm()) {
            event.preventDefault();
        }
    });
}

// Input events
if (username)
    username.addEventListener('input', validateUsername);

if (password)
    password.addEventListener('input', () => validatePassword(password, passwordErr));

if (newPassword)
    newPassword.addEventListener('input', () => {
        validatePassword(newPassword, newPasswordErr);

        if (confirmPassword && confirmPassword.value)
            validateConfirmPassword();
    });

if (confirmPassword)
    confirmPassword.addEventListener('input', validateConfirmPassword);

// Submit validation
function validateLoginForm() {
    const isUsernameValid = validateUsername();
    const isPasswordValid = validatePassword(password, passwordErr);

    return isUsernameValid && isPasswordValid;
}

function validateForgotPasswordForm() {
    return validateUsername();
}

function validateResetPasswordForm() {
    const isNewPasswordValid = validatePassword(newPassword, newPasswordErr);
    const isConfirmPasswordValid = validateConfirmPassword();

    return isNewPasswordValid && isConfirmPasswordValid;
}

// Username validation
function validateUsername() {
    const isValid = username.value.trim().length >= 3;

    username.classList.toggle('is-valid', isValid);
    username.classList.toggle('is-invalid', !isValid);

    usernameErr.textContent = isValid
        ? ""
        : "Username must be at least 3 characters.";

    return isValid;
}

// Password validation
function validatePassword(input, errorElement) {
    const value = input.value;

    if (value.length < 6) {
        input.classList.remove('is-valid');
        input.classList.add('is-invalid');

        errorElement.textContent =
            "Password must be greater than 5 characters.";

        return false;
    }

    const isValid =
        /[A-Z]/.test(value) &&
        /[0-9]/.test(value) &&
        /[!@#$%^&*()]/.test(value);

    if (!isValid) {
        input.classList.remove('is-valid');
        input.classList.add('is-invalid');

        errorElement.textContent =
            "Password must contain at least 1 uppercase letter, 1 number, and 1 special character.";

        return false;
    }

    input.classList.remove('is-invalid');
    input.classList.add('is-valid');

    errorElement.textContent = "";

    return true;
}

// Confirm password validation
function validateConfirmPassword() {
    const isValid =
        confirmPassword.value.trim() !== "" &&
        confirmPassword.value === newPassword.value;

    confirmPassword.classList.toggle('is-valid', isValid);
    confirmPassword.classList.toggle('is-invalid', !isValid);

    confirmPasswordErr.textContent = isValid
        ? ""
        : "Passwords do not match.";

    return isValid;
}