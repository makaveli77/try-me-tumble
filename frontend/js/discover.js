const discoverBtn = document.getElementById('discoverBtn');
const nextBtn = document.getElementById('nextBtn');
const previewCard = document.getElementById('previewCard');
const iframeModal = document.getElementById('iframeModal');
const stumbleFrame = document.getElementById('stumbleFrame');
const currentUrlText = document.getElementById('currentUrl');
const openNewTab = document.getElementById('openNewTab');
const errorOpenNewTab = document.getElementById('errorOpenNewTab');
const iframeLoader = document.getElementById('iframeLoader');
const iframeError = document.getElementById('iframeError');

// Action buttons
const upvoteBtn = document.getElementById('upvoteBtn');
const saveBtn = document.getElementById('saveBtn');
const upvoteCountText = document.getElementById('upvoteCount');
const reportModal = document.getElementById('reportModal');
const reportReason = document.getElementById('reportReason');

let stumbleTimeout;
let currentWebsiteId = null;

// Helpers
function getAuthHeaders() {
    // Rely on cookies for auth, just set Content-Type
    return { 'Content-Type': 'application/json' };
}

// User state check on load to switch out login/signup buttons
async function checkAuthStatus() {
    try {
        const response = await fetch('/api/Auth/me', { headers: getAuthHeaders() });
        const logoutBtn = document.getElementById('logoutBtn');
        const loginLink = document.querySelector('a[href="login.html"]');
        const signupLink = document.querySelector('a[href="signup.html"]');
        
        if (response.ok) {
            // User is authenticated, hide login/signup, show logout
            if(loginLink) loginLink.classList.add('hidden');
            if(signupLink) signupLink.classList.add('hidden');
            if(logoutBtn) {
                logoutBtn.classList.remove('hidden');
                logoutBtn.addEventListener('click', async () => {
                    await fetch('/api/Auth/logout', { method: 'POST', headers: getAuthHeaders() });
                    window.location.reload();
                });
            }
        }
    } catch (err) {
        console.error("Auth check failed", err);
    }
}
checkAuthStatus();

function requireAuth() {
    // With cookie logic, we might not always know if they're logged in client-side
    // but ideally the backend will return 401 which we catch. 
    // If you have a logged-in state cookie or variable, check it here:
    // for now we'll let the 401 handle the rejection.
    return true;
}

// Actions
async function toggleUpvote() {
    if (!requireAuth() || !currentWebsiteId) return;

    try {
        const response = await fetch(`/api/Websites/${currentWebsiteId}/upvote`, {
            method: 'POST',
            headers: getAuthHeaders()
        });

        if (response.ok) {
            const upcoteIcon = upvoteBtn.querySelector('svg');
            const data = await response.json();
            
            // Toggle UI state
            if (upvoteBtn.classList.contains('text-pink-500')) {
                upvoteBtn.classList.remove('text-pink-500');
                let count = parseInt(upvoteCountText.textContent || '0');
                if (count > 0) upvoteCountText.textContent = count - 1;
                upcoteIcon.setAttribute('fill', 'none');
            } else {
                upvoteBtn.classList.add('text-pink-500');
                upvoteCountText.textContent = parseInt(upvoteCountText.textContent || '0') + 1;
                upcoteIcon.setAttribute('fill', 'currentColor');
            }
        } else if (response.status === 401) {
            alert('Please log in to upvote websites.');
        }
    } catch (err) {
        console.error('Failed to upvote', err);
    }
}

async function toggleSave() {
    if (!requireAuth() || !currentWebsiteId) return;

    try {
        const response = await fetch(`/api/Websites/${currentWebsiteId}/save`, {
            method: 'POST',
            headers: getAuthHeaders()
        });

        if (response.ok) {
            const saveIcon = saveBtn.querySelector('svg');
            // Toggle UI state
            if (saveBtn.classList.contains('text-blue-500')) {
                saveBtn.classList.remove('text-blue-500');
                saveIcon.setAttribute('fill', 'none');
            } else {
                saveBtn.classList.add('text-blue-500');
                saveIcon.setAttribute('fill', 'currentColor');
            }
        } else if (response.status === 401) {
            alert('Please log in to save websites to your profile.');
        }
    } catch (err) {
        console.error('Failed to save', err);
    }
}

window.openReportModal = function() {
    if (!requireAuth() || !currentWebsiteId) return;
    reportReason.value = '';
    reportModal.classList.remove('hidden');
};

window.closeReportModal = function() {
    reportModal.classList.add('hidden');
};

window.submitReport = async function(event) {
    event.preventDefault();
    if (!currentWebsiteId) return;

    const reason = reportReason.value.trim();
    if (!reason) return;

    try {
        const response = await fetch(`/api/Websites/${currentWebsiteId}/report`, {
            method: 'POST',
            headers: getAuthHeaders(),
            body: JSON.stringify({ reason })
        });

        if (response.ok) {
            alert('Website reported successfully.');
            closeReportModal();
            stumble(); // Skip to next site automatically after report
        } else if (response.status === 401) {
            alert('You must be logged in to report content.');
            closeReportModal();
        } else {
            alert('Failed to report website.');
        }
    } catch (err) {
        console.error('Failed to report', err);
    }
};

async function stumble() {
    // Cancel any previous timeout
    if (stumbleTimeout) clearTimeout(stumbleTimeout);

    // Show skeleton loader on initial click
    if (iframeModal.classList.contains('hidden')) {
        previewCard.classList.remove('hidden');
        setTimeout(() => {
            previewCard.classList.remove('opacity-0', 'translate-y-4');
        }, 50);
    }

    // Prepare iframe
    iframeLoader.classList.remove('hidden');
    iframeError.classList.add('hidden');
    stumbleFrame.classList.add('opacity-0');
    stumbleFrame.src = 'about:blank';

    try {
        const response = await fetch('/api/Websites/discover', {
            headers: getAuthHeaders()
        });
        if (response.status === 404) {
            alert("No websites found! Please seed the database first.");
            closeModal();
            return;
        }
        if (!response.ok) throw new Error('Discovery failed');
        
        const data = await response.json();
        
        if (data && data.url) {
            currentWebsiteId = data.id; // Store current ID
            
            // Set UI state for upvotes/saves
            upvoteCountText.textContent = data.upvotesCount || 0;
            
            const upcoteIcon = upvoteBtn.querySelector('svg');
            if (data.isUpvotedByCurrentUser) {
                upvoteBtn.classList.add('text-pink-500');
                upcoteIcon.setAttribute('fill', 'currentColor');
            } else {
                upvoteBtn.classList.remove('text-pink-500');
                upcoteIcon.setAttribute('fill', 'none');
            }

            const saveIcon = saveBtn.querySelector('svg');
            if (data.isSavedByCurrentUser) {
                saveBtn.classList.add('text-blue-500');
                saveIcon.setAttribute('fill', 'currentColor');
            } else {
                saveBtn.classList.remove('text-blue-500');
                saveIcon.setAttribute('fill', 'none');
            }

            // Open in custom frame
            iframeModal.classList.remove('hidden');
            document.body.style.overflow = 'hidden'; // Prevent scrolling
            
            stumbleFrame.src = data.url;
            currentUrlText.textContent = data.url;
            openNewTab.href = data.url;
            errorOpenNewTab.href = data.url;

            // Timeout fallback for unresponsive loads (CSP/X-Frame-Options)
            stumbleTimeout = setTimeout(() => {
                if (!stumbleFrame.classList.contains('opacity-0')) return; // Already loaded
                iframeLoader.classList.add('hidden');
                iframeError.classList.remove('hidden');
            }, 8000); // 8 second timeout

            // Handle frame loading
            stumbleFrame.onload = () => {
                clearTimeout(stumbleTimeout);
                iframeLoader.classList.add('hidden');
                iframeError.classList.add('hidden');
                stumbleFrame.classList.remove('opacity-0');
            };

            console.log("Stumbling to:", data.url);
        } else {
            alert("No websites found! Check back later.");
            closeModal();
        }
    } catch (err) {
        console.error("Discovery failed", err);
        alert("Something went wrong while trying to tumble. Please try again.");
        closeModal();
    }
}

function closeModal() {
    iframeModal.classList.add('hidden');
    document.body.style.overflow = 'auto';
    stumbleFrame.src = 'about:blank';
    previewCard.classList.add('hidden', 'opacity-0', 'translate-y-4');
}

discoverBtn.addEventListener('click', stumble);
nextBtn.addEventListener('click', stumble);
upvoteBtn.addEventListener('click', toggleUpvote);
saveBtn.addEventListener('click', toggleSave);
