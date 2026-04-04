document.getElementById('loginForm').addEventListener('submit', async (e) => {
    e.preventDefault();
    
    const username = document.getElementById('username').value.trim();
    const password = document.getElementById('password').value.trim();
    const errorMsg = document.getElementById('errorMsg');
    
    try {
        const response = await fetch('/api/Auth/login', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ username, password })
        });

        if (response.ok) {
            const data = await response.json();
            // Automatically redirect to the discovery page on successful login
            window.location.href = 'discover.html';
        } else {
            const text = await response.text();
            errorMsg.textContent = text || 'Invalid username or password.';
            errorMsg.classList.remove('hidden');
        }
    } catch (err) {
        console.error('Login error:', err);
        errorMsg.textContent = 'Oops! An unexpected error occurred. Is the backend running?';
        errorMsg.classList.remove('hidden');
    }
});