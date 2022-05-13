//IMPORTANT: this production environment is suitable only for Cloud on Azure, not for local K8s cluster


export const environment = {
  production: true,
  appVersion: require('../../package.json').version,
  apiUrl: "https://20.76.142.47:5501/api/",
  hubUrl: "http://20.54.180.122:5001/hubs/",  //must have valid certificate to enable signalr on https 
  dateTimeFormat: 'dd/MM/yyyy - HH:mm:ss',
};
