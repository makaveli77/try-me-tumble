document.getElementById('signupForm').addEventListener('submit', async (e) => {
    e.preventDefault();
    
    const username = document.getElementById('username').value.trim();
    const email = document.getElementById('email').value.trim();
    const password = document.getElementById('password').value.trim();
    
    const errorMsg = document.getElementById('errorMsg');
    const submitBtn = document.getElementById('submitBtn');
    
    errorMsg.classList.add('hidden');
    submitBtn.textContent = 'Joining...';
    submitBtn.disabled = true;

    try {
        const response = await fetch('/api/Auth/register', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ username, email, password })
        });

        if (response.ok) {
            // Instantly trigger login after a successful registration to grab the secure HttpOnly cookie
            const loginResp = await fetch('/api/Auth/login', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ username, password })
            });

            if (loginResp.ok) {
                window.location.href = 'discover.html'; // Send them straight to discovering
            } else {
                window.location.href = 'login.html'; // Fallback if login fails
            }
        } else {
            const text = await response.text();
            errorMsg.textContent = text || 'Failed to create user. Perhaps username or email already exists.';
            errorMsg.classList.remove('hidden');
        }
    } catch (err) {
        console.error('Signup error:', err);
        errorMsg.textContent = 'Oops! An unexpected error occurred. Is the backend active?';
        errorMsg.classList.remove('hidden');
    } finally {
        submitBtn.textContent = 'Sign Up';
        submitBtn.disabled = false;
    }
});