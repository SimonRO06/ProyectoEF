document.getElementById("signUpForm").addEventListener("submit", function(event) {
    event.preventDefault();  // Evitar que el formulario se envíe de forma predeterminada

    // Limpiar los mensajes de error previos
    clearErrorMessages();

    // Obtener los valores de los campos
    const username = document.getElementById("username").value.trim();
    const email = document.getElementById("email").value.trim();
    const password = document.getElementById("password").value.trim();
    const role = document.getElementById("role").value;

    // Bandera para verificar si el formulario es válido
    let valid = true;

    // Validación de los campos
    if (!username) {
        showErrorMessage("username", "El nombre de usuario es obligatorio.");
        valid = false;
    }

    if (!email) {
        showErrorMessage("email", "El correo electrónico es obligatorio.");
        valid = false;
    } else if (!validateEmail(email)) {
        showErrorMessage("email", "Por favor, ingrese un correo electrónico válido.");
        valid = false;
    }

    if (!password) {
        showErrorMessage("password", "La contraseña es obligatoria.");
        valid = false;
    }

    if (!role) {
        showErrorMessage("role", "Debe seleccionar un rol.");
        valid = false;
    }

    // Si algún campo es inválido, no enviamos el formulario
    if (!valid) {
        return;
    }

    // Si todo es válido, enviamos los datos de registro al backend
    fetch("https://localhost:5001/api/users/register", {
    method: "POST",
    headers: {
        "Content-Type": "application/json"
    },
    body: JSON.stringify({
        email: email,
        username: username,
        password: password
    })
    })
    .then(response => {
        if (!response.ok) {
        return response.text().then(text => { throw new Error(text); });
        }
        return response.text();
    })
    .then(message => {
        alert("Registro exitoso ✅");
        console.log("Respuesta del servidor:", message);
        window.location.href = "login.html";
    })
    .catch(error => {
        alert("Error al registrarse: " + error.message);
    });

});

/* // Función para validar el formato del email
function validateEmail(email) {
    const regex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return regex.test(email);
}

// Función para mostrar un mensaje de error junto al campo
function showErrorMessage(fieldId, message) {
    const field = document.getElementById(fieldId);
    const errorMessage = document.createElement('div');
    errorMessage.classList.add('error-message');
    errorMessage.textContent = message;
    field.parentNode.appendChild(errorMessage);
}

// Función para limpiar los mensajes de error
function clearErrorMessages() {
    const errorMessages = document.querySelectorAll('.error-message');
    errorMessages.forEach(msg => msg.remove());
} */