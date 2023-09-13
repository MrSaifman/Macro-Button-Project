function handleIdleLighting() {
    setActiveButton('idle-lighting', 'Idle Lighting');
}

function handleButtonPressLighting() {
    setActiveButton('button-press-lighting', 'Button Press Lighting');
}

function handleLidLiftLighting() {
    setActiveButton('lid-lift-lighting', 'Lid Lift Lighting');
}

function setActiveButton(buttonId, titleText) {
    const selectorButtons = document.querySelectorAll('.selector-button');
    selectorButtons.forEach(btn => {
        btn.classList.remove('active');
    });
    document.getElementById(buttonId).classList.add('active');
    document.getElementById('selectedTitle').textContent = titleText;
}
