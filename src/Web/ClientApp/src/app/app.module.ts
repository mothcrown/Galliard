import { BrowserModule } from '@angular/platform-browser';
import { APP_ID, NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { HTTP_INTERCEPTORS, provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';

import { ModalModule } from 'ngx-bootstrap/modal';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { CounterComponent } from './counter/counter.component';
import { FetchDataComponent } from './fetch-data/fetch-data.component';
import { UploadAudioComponent } from './upload-audio/upload-audio.component';
import { AuthorizeInterceptor } from 'src/api-authorization/authorize.interceptor';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NovelizeProcessComponent } from "./novelize-process/novelize-process.component";
import { FileStorageService } from "./services/file-storage.service";
import { ProcessStageComponent } from "./process-stage/process-stage.component";

@NgModule({
    declarations: [
        AppComponent,
        NavMenuComponent,
        HomeComponent,
        CounterComponent,
        FetchDataComponent,
        UploadAudioComponent,
        NovelizeProcessComponent,
        ProcessStageComponent
    ],
    bootstrap: [AppComponent],
    imports: [
        BrowserModule,
        FormsModule,
        RouterModule.forRoot([
            { path: '', component: UploadAudioComponent, pathMatch: 'full' },
            { path: 'novelize', component: NovelizeProcessComponent }
        ]),
        BrowserAnimationsModule,
        ModalModule.forRoot()],
    providers: [
        { provide: APP_ID, useValue: 'ng-cli-universal' },
        { provide: HTTP_INTERCEPTORS, useClass: AuthorizeInterceptor, multi: true },
        provideHttpClient(withInterceptorsFromDi()),
        FileStorageService
    ]
})
export class AppModule { }
