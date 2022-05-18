
export const environment = {
  production: true,
  appVersion: require('../../package.json').version,
  apiUrl: "https://localhost:5501/api/",
  hubUrl: "https://localhost:5001/hubs/",  //must have valid certificate to enable signalr on https 
  dateTimeFormat: 'dd/MM/yyyy - HH:mm:ss',
  kubernetesDashboard: "http://localhost:8001/api/v1/namespaces/kubernetes-dashboard/services/https:kubernetes-dashboard:/proxy/#/login"
};
