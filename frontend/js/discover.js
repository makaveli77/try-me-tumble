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

let stumbleTimeout;

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
        const response = await fetch('/api/Websites/discover');
        if (response.status === 404) {
            alert("No websites found! Please seed the database first.");
            closeModal();
            return;
        }
        if (!response.ok) throw new Error('Discovery failed');
        
        const data = await response.json();
        
        if (data && data.url) {
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
