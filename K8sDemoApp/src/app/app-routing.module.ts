import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './Func_Login/login.component';
import { LogsviewerComponent } from './Func_Logs/logsviewer/logsviewer.component';
import { ClusterMonitoringComponent } from './Pages/clusterMonitoring/cluster-monitoring.component';
import { HomeComponent } from './Pages/home/home.component';
import { AsyncJobsTestComponent } from './Pages/SystemTest/async-jobs-test/async-jobs-test.component';
import { UsersComponent } from './Pages/UserManagement/users/users.component';


const routes: Routes = [
  { path: '', component: HomeComponent, pathMatch: 'full'},
  { path: 'login', component: LoginComponent},
  { path: 'asyncJobTest', component: AsyncJobsTestComponent},
  { path: 'clusterMonitoring', component: ClusterMonitoringComponent},
  { path: 'usermanagement', component: UsersComponent},
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule {



}
