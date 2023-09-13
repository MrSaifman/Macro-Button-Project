document.addEventListener('DOMContentLoaded', function() {
    // Get all clickable elements
    const clickableItems = document.querySelectorAll('.clickable');
    const contentDiv = document.getElementById('content');

    clickableItems.forEach(item => {
        // On click, add the active class to the clicked item, remove it from others, and load the respective content
        item.addEventListener('click', function() {
            clickableItems.forEach(i => {
                i.classList.remove('active');
            });
            this.classList.add('active');
            this.classList.add('bop-animation'); // Add the bop animation class

            // Remove the bop-animation class after 300ms (duration of the animation)
            setTimeout(() => {
                this.classList.remove('bop-animation');
            }, 300);

            // Load the respective content based on the clicked item
            let pageToLoad = '';
            switch (this.textContent.trim()) {
                case 'Dashboard':
                    pageToLoad = 'dashboard.html';
                    break;
                case 'Macros':
                    pageToLoad = 'macros.html';
                    break;
                case 'Lighting':
                    pageToLoad = 'lighting.html';
                    break;
                case 'Help':
                    pageToLoad = 'help.html';
                    break;
                case 'Settings':
                    pageToLoad = 'settings.html';
                    break;
            }
            
            if (pageToLoad) {
                fetch(pageToLoad)
                    .then(response => response.text())
                    .then(data => {
                        contentDiv.innerHTML = data;
                    })
                    .catch(error => {
                        console.error('Error loading page:', error);
                    });
            }
        });
    });

    // Load the Dashboard content by default on startup
    fetch('dashboard.html')
        .then(response => response.text())
        .then(data => {
            contentDiv.innerHTML = data;
            document.querySelector('.clickable').classList.add('active'); // Set the Dashboard button as active
        })
        .catch(error => {
            console.error('Error loading default page:', error);
        });
});
