let openedPopup = false;
const levels = [simpleLevelPlan1, simpleLevelPlan2, simpleLevelPlan3];
const openPopup = document.getElementById('openPopup');
const startP1 = document.getElementById('startP1');
const startP2 = document.getElementById('startP2');
const popup = document.getElementById('popup');

openPopup.addEventListener('click', () => {
    popup.style.display = 'flex';
    openPopup.style.display = 'none';
    openedPopup = true;
    runGame([], DOMDisplay);
});

startP1.addEventListener('click', () => {
    popup.style.display = 'none';
    openPopup.style.display = 'inline-block';
    openedPopup = false;
    runGame(levels, DOMDisplay);
});

startP2.addEventListener('click', () => {
    popup.style.display = 'none';
    openPopup.style.display = 'inline-block';
    openedPopup = false;
    runGame(levels, DOMDisplay);
})