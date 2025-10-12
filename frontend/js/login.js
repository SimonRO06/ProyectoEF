document.addEventListener("DOMContentLoaded", () => {
    const ROLE_REDIRECT = {
    "Administrador": "usuarios.html",
    "Recepcionista": "recepcionista.html",
    "Mecanico": "mecanico.html"
    };

    document.getElementById("loginForm").addEventListener("submit", async function (event) {
        event.preventDefault();

        const username = document.getElementById("username").value.trim(); // usa el campo email como username visual
        const password = document.getElementById("password").value.trim();

        if (!username || !password) {
            alert("Por favor, complete todos los campos.");
            return;
        }

        try {
            const response = await fetch(`http://localhost:5000/api/users/login`, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ username, password })
            });

            if (!response.ok) {
                const errorText = await response.text();
                throw new Error(errorText || "Error en el inicio de sesión");
            }

            const data = await response.json();

            // ✅ Guardar token y refresh token
            localStorage.setItem("token", data.token);
            localStorage.setItem("refreshToken", data.refreshToken);
            localStorage.setItem("username", data.userName);

            // ✅ Si tu backend devuelve roles como array
            const roles = data.roles || [];
            if (roles.length === 0) {
                alert("No se detectó ningún rol para este usuario.");
                return;
            }

            const role = roles[0];
            localStorage.setItem("role", role);

            // ✅ Redirigir según el rol
            const redirectPage = ROLE_REDIRECT[role] || "main.html";
            alert(`Bienvenido ${data.userName} (${role})`);
            window.location.href = redirectPage;

        } catch (error) {
            alert("Error al iniciar sesión: " + error.message);
        }
    });
});
