const { app, BrowserWindow } = require('electron');

let mainWindow;

app.on('ready', () => {
    mainWindow = new BrowserWindow({
        width: 800,
        height: 600,
        backgroundColor: '#141415',
        webPreferences: {
          nodeIntegration: true,
          contextIsolation: false,
          enableRemoteModule: true
        }
    });

  // Hide the menu bar
  //mainWindow.setMenuBarVisibility(false);

  mainWindow.loadFile('index.html');

//   document.querySelector('.nav-item button').addEventListener('click', function() {
//     // Your code here, e.g., navigate to a different page or open a modal
//     });

  mainWindow.on('closed', () => {
    mainWindow = null;
  });
  
});
