
export const environment = {
  production: true,
  appVersion: require('../../package.json').version,
  apiUrl: "https://localhost:5501/api/",
  hubUrl: "http://localhost:5001/hubs/",  //must have valid certificate to enable signalr on https 
  dateTimeFormat: 'dd/MM/yyyy - HH:mm:ss',
};
