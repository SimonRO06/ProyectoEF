document.getElementById("loginForm").addEventListener("submit", function(event) {
    event.preventDefault();  

    // Obtener los valores de los campos
    const email = document.getElementById("email").value.trim();
    const password = document.getElementById("password").value.trim();

    // Validación simple
    let valid = true;
    
    // Validación de campos vacíos
    if (!email || !password) {
        alert("Por favor, complete todos los campos.");
        valid = false;
    }

    if (!valid) {
        return; // No enviar el formulario si hay un campo vacío
    }

    fetch("https://localhost:5001/api/users/login", {
    method: "POST",
    headers: {
        "Content-Type": "application/json"
    },
    body: JSON.stringify({
        username: username,
        password: password
    })
    })
    .then(response => {
        if (!response.ok) {
        return response.text().then(text => { throw new Error(text); });
        }
        return response.json();
    })
    .then(data => {
        if (data.isAuthenticated) {
        localStorage.setItem("token", data.token);
        alert("Login exitoso 🎉 Bienvenido " + data.userName);
        window.location.href = "main.html";
        } else {
        alert("Error: " + data.message);
        }
    })
    .catch(error => {
        alert("Error al iniciar sesión: " + error.message);
    });

});