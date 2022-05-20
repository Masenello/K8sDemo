import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HttpClientModule, HttpClient, HTTP_INTERCEPTORS } from '@angular/common/http';
import { NavComponent } from './Func_Navigation/nav/nav.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { FormsModule } from '@angular/forms';
import { ToastrModule } from 'ngx-toastr';
import {LogMonitorModule} from 'ngx-log-monitor';
import { LoginComponent } from './Func_Login/login.component';

import {MatCardModule} from '@angular/material/card';
import {MatFormFieldModule} from '@angular/material/form-field';
import {MatToolbarModule} from '@angular/material/toolbar';
import {MatIconModule} from '@angular/material/icon';
import {MatInputModule} from '@angular/material/input';
import {MatButtonModule} from '@angular/material/button';
import {MatTableModule} from '@angular/material/table';
import {MatPaginatorModule} from '@angular/material/paginator';
import {MatSortModule} from '@angular/material/sort';
import {MatSidenavModule} from '@angular/material/sidenav';
import {MatListModule} from '@angular/material/list';
import {MatMenuModule} from '@angular/material/menu';
import {MatSnackBarModule} from '@angular/material/snack-bar';
import {MatDialogModule} from '@angular/material/dialog';
import {MatTooltipModule} from '@angular/material/tooltip';
import {MatSlideToggleModule} from '@angular/material/slide-toggle';
import {MatDividerModule} from '@angular/material/divider';
import {MatAutocompleteModule} from '@angular/material/autocomplete';
import {MatSelectModule} from '@angular/material/select';
import {MatProgressSpinnerModule} from '@angular/material/progress-spinner';
import {MatTabsModule} from '@angular/material/tabs';
import {MatCheckboxModule} from '@angular/material/checkbox';
import {MatButtonToggleModule} from '@angular/material/button-toggle';
import {MatProgressBarModule} from '@angular/material/progress-bar';
import {MatGridListModule} from '@angular/material/grid-list';

import { LogsviewerComponent } from './Func_Logs/logsviewer/logsviewer.component';
import { JobmanagerComponent } from './Func_Jobs/jobmanager/jobmanager.component';
import { HomeComponent } from './Pages/home/home.component';
import { ClusterMonitoringComponent } from './Pages/clusterMonitoring/cluster-monitoring.component';
import { AsyncJobsTestComponent } from './Pages/SystemTest/async-jobs-test/async-jobs-test.component';
import { JobstatusComponent } from './Func_Jobs/jobstatus/jobstatus.component';
import { JobStatusEnumNamePipe, JobTypeEnumNamePipe } from './Func_Jobs/jobEnumsPipes';
import { AuthorizationInterceptor } from './Func_Login/authorization.interceptor';
import { LogViewerPipe } from './Func_Logs/log-viewer.pipe';
import { JobhistoricalComponent } from './Func_Jobs/jobhistorical/jobhistorical.component';
import { GuiGridModule } from '@generic-ui/ngx-grid';
import { NgxEchartsModule } from 'ngx-echarts';
import { DirectorStatusChartComponent } from './Func_Jobs/director-status-chart/director-status-chart.component';
import { ScrollingModule } from '@angular/cdk/scrolling';
import { DirectorSetParametersComponent } from './Func_Jobs/director-set-parameters/director-set-parameters.component';
import { DirectorStatusLoadComponent } from './Func_Jobs/director-status-load/director-status-load.component';
import { NgxGaugeModule } from 'ngx-gauge';
import { PodsInfoComponent } from './Func_k8s/pods-info/pods-info.component';
import { PodstatusComponent } from './Func_k8s/podstatus/podstatus.component';
import { PodUtils } from './_shared/Utils/PodUtils';
import { PodlogsviewerComponent } from './Func_k8s/podlogsviewer/podlogsviewer.component';
import { AwaiterPopUpComponent } from './_shared/Components/awaiter-pop-up/awaiter-pop-up.component';
import { HttpLoadingInterceptor } from './_shared/Interceptors/http-loading.interceptor';
import { NotAllowedComponent } from './Pages/Generic/not-allowed/not-allowed.component';
import { NotFoundComponent } from './Pages/Generic/not-found/not-found.component';
import { NotWorkingComponent } from './Pages/Generic/not-working/not-working.component';
import { SystemArchitectureComponent } from './Pages/SystemTest/system-architecture/system-architecture.component';
import { UsersManagementComponent } from './Pages/UserManagement/users-management/users-management.component';
import { UsersComponent } from './Func_Users/users/users.component';
import { RoleToLongStringPipe, RoleToShortStringPipe } from './_shared/Pipes/role-tostring.pipe';
import { DialogContent } from './_shared/Services/dialog.service';


@NgModule({
  declarations: [
    AppComponent,
    NavComponent,
    LoginComponent,
    LogsviewerComponent,
    JobmanagerComponent,
    HomeComponent,
    ClusterMonitoringComponent,
    AsyncJobsTestComponent,
    JobstatusComponent,
    JobStatusEnumNamePipe,
    JobTypeEnumNamePipe,
    LogViewerPipe,
    JobhistoricalComponent,
    DirectorStatusChartComponent,
    DirectorSetParametersComponent,
    DirectorStatusLoadComponent,
    PodsInfoComponent,
    PodstatusComponent,
    PodlogsviewerComponent,
    AwaiterPopUpComponent,
    NotAllowedComponent,
    NotFoundComponent,
    NotWorkingComponent,
    SystemArchitectureComponent,
    UsersManagementComponent,
    UsersComponent,
    RoleToShortStringPipe,
    RoleToLongStringPipe,
    DialogContent,
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    FormsModule,
    ToastrModule.forRoot({
      positionClass: "toast-bottom-right"
    }),
    LogMonitorModule,
    MatCardModule,
    MatFormFieldModule,
    MatToolbarModule,
    MatIconModule,
    MatInputModule,
    MatButtonModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatSidenavModule,
    MatListModule,
    MatMenuModule,
    MatSnackBarModule,
    MatDialogModule,
    MatTooltipModule,
    MatSlideToggleModule,
    MatDividerModule,
    MatAutocompleteModule,
    MatSelectModule,
    MatProgressSpinnerModule,
    MatTabsModule,
    MatCheckboxModule,
    MatButtonToggleModule,
    MatProgressBarModule,
    MatGridListModule,
    GuiGridModule,
    NgxEchartsModule.forRoot({
      echarts: () => import('echarts')
    }),
    ScrollingModule,
    NgxGaugeModule


  ],
  providers: [
    LogViewerPipe,
    JobStatusEnumNamePipe,
    JobTypeEnumNamePipe,
    RoleToShortStringPipe,
    RoleToLongStringPipe,
    PodUtils,
    { provide: HTTP_INTERCEPTORS, useClass: AuthorizationInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: HttpLoadingInterceptor, multi: true },
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
