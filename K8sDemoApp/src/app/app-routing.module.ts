import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './Func_Login/login.component';
import { ClusterMonitoringComponent } from './Pages/clusterMonitoring/cluster-monitoring.component';
import { NotFoundComponent } from './Pages/Generic/not-found/not-found.component';
import { HomeComponent } from './Pages/home/home.component';
import { AsyncJobsTestComponent } from './Pages/SystemTest/async-jobs-test/async-jobs-test.component';
import { SystemArchitectureComponent } from './Pages/SystemTest/system-architecture/system-architecture.component';
import { UsersManagementComponent } from './Pages/UserManagement/users-management/users-management.component';


const routes: Routes = [
  { path: 'home', component: HomeComponent},
  { path: 'login', component: LoginComponent},
  { path: 'asyncJobTest', component: AsyncJobsTestComponent},
  { path: 'clusterMonitoring', component: ClusterMonitoringComponent},
  { path: 'usermanagement', component: UsersManagementComponent},
  { path: 'systemArchitecture', component: SystemArchitectureComponent},
  { path: 'not-found', component: NotFoundComponent},
  { path: '**', redirectTo: '/not-found' },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule {



}
