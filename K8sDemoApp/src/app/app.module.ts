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

import { LogsviewerComponent } from './Func_Logs/logsviewer/logsviewer.component';
import { JobmanagerComponent } from './Func_Jobs/jobmanager/jobmanager.component';
import { HomeComponent } from './Pages/home/home.component';
import { ClusterMonitoringComponent } from './Pages/clusterMonitoring/cluster-monitoring.component';
import { DatabaseTestComponent } from './Pages/SystemTest/database-test/database-test.component';
import { AsyncJobsTestComponent } from './Pages/SystemTest/async-jobs-test/async-jobs-test.component';
import { JobstatusComponent } from './Func_Jobs/jobstatus/jobstatus.component';
import { JobStatusEnumNamePipe, JobTypeEnumNamePipe } from './Func_Jobs/jobEnumsPipes';
import { AuthorizationInterceptor } from './Func_Login/authorization.interceptor';
import { LogViewerPipe } from './Func_Logs/log-viewer.pipe';
import { JobhistoricalComponent } from './Func_Jobs/jobhistorical/jobhistorical.component';
import { GuiGridModule } from '@generic-ui/ngx-grid';
import { DirectorStatusViewerComponent } from './Func_DirectorStatus/director-status-viewer/director-status-viewer.component';

@NgModule({
  declarations: [
    AppComponent,
    NavComponent,
    LoginComponent,
    LogsviewerComponent,
    JobmanagerComponent,
    HomeComponent,
    ClusterMonitoringComponent,
    DatabaseTestComponent,
    AsyncJobsTestComponent,
    JobstatusComponent,
    JobStatusEnumNamePipe,
    JobTypeEnumNamePipe,
    LogViewerPipe,
    JobhistoricalComponent,
    DirectorStatusViewerComponent,
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
    GuiGridModule,


  ],
  providers: [
    LogViewerPipe,
    JobStatusEnumNamePipe,
    JobTypeEnumNamePipe,
    { provide: HTTP_INTERCEPTORS, useClass: AuthorizationInterceptor, multi: true },
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
