import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './Func_Login/login.component';
import { LogsviewerComponent } from './Func_Logs/logsviewer/logsviewer.component';
import { ClusterMonitoringComponent } from './Pages/clusterMonitoring/cluster-monitoring.component';
import { HomeComponent } from './Pages/home/home.component';
import { AsyncJobsTestComponent } from './Pages/SystemTest/async-jobs-test/async-jobs-test.component';
import { DatabaseTestComponent } from './Pages/SystemTest/database-test/database-test.component';


const routes: Routes = [
  { path: '', component: HomeComponent, pathMatch: 'full'},
  { path: 'login', component: LoginComponent},
  { path: 'databaseTest', component: DatabaseTestComponent},
  { path: 'asyncJobTest', component: AsyncJobsTestComponent},
  { path: 'clusterMonitoring', component: ClusterMonitoringComponent},
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule {



}
