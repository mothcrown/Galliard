import {Component} from '@angular/core';
import { FileStorageService } from "../services/file-storage.service";
import { NovelizeClient, NovelizeCommand, API_BASE_URL } from "../web-api-client";
import { ProcessStage } from "../process-stage/process-stage";
import { HubConnection, HubConnectionBuilder, LogLevel } from "@microsoft/signalr";
import * as signalR from '@microsoft/signalr';

@Component({
  selector: 'app-novelize-process',
  templateUrl: './novelize-process.component.html',
  styleUrls: ['./novelize-process.component.scss']
})

export class NovelizeProcessComponent {

  private novelizeHub: HubConnection
  stages: ProcessStage[] = [];

  constructor(
    private fileStorageService: FileStorageService,
    private novelizeClient: NovelizeClient
  ) {}

  ngOnInit() {
    this.startHubConnection();
    this.sendFile();
  }

  private startHubConnection() {
    console.log("Starting Hub connection...");
    this.novelizeHub = new HubConnectionBuilder()
      .withUrl('https://localhost:5001/NovelizeHub', {withCredentials: true})
      .configureLogging(signalR.LogLevel.Information)
      .build();
    this.novelizeHub.start()
      .then(() => console.log('connection started'))
      .catch(err => console.log(err));
    this.addListeners();
  }

  private sendFile() {
    let file = this.fileStorageService.getFile();
    let fileName = file.name;
    let novelizeClient = this.novelizeClient;

    this.stages.push({
      message: 'Uploading...',
      loading: true,
      success: false
    });

    const reader = new FileReader();
    reader.readAsDataURL(file);
    reader.onload = () => {
      let novelizeCommand = new NovelizeCommand();
      novelizeCommand.fileName = fileName;
      novelizeCommand.contents = reader.result.toString();
      let subscription = novelizeClient.novelizeProcess(novelizeCommand).subscribe(
        result => {
          console.log(result);
          this.stages[0].message = "Audio saved";
          this.stages[0].loading = false;
          this.stages[0].success = true;
          this.stages.push({
            message: 'Transcribing audio...',
            loading: true,
            success: false
          });
        },
        error => {
          console.error(error);
          this.stages[0].loading = false;
          this.stages[0].success = false;
        }
      );
    };
  }

  private addListeners() {
    this.novelizeHub.on("AudioTranscribed", (message: string) => {
      if (message == "OK") {
        this.stages[1].message = "Audio transcribed";
        this.stages[1].loading = false;
        this.stages[1].success = true;
        this.stages.push({
          message: 'Novelizing transcription...',
          loading: true,
          success: false
        });
      } else {
        this.stages[1].loading = false;
        this.stages[1].success = false;
      }

    });

    this.novelizeHub.on("TranscriptionNovelized", (message: string) => {
      if (message == "OK") {
        this.stages[2].message = "Transcription novelized";
        this.stages[2].loading = false;
        this.stages[2].success = true;
      } else {
        this.stages[2].loading = false;
        this.stages[2].success = false;
      }
    })
  }

  private arrayBufferToBase64(buffer: ArrayBuffer) : string {
    let binary = '';
    let bytes = new Uint8Array(buffer);
    let len = bytes.byteLength;
    for (let i = 0; i < len; i++) {
      binary += String.fromCharCode(bytes[i]);
    }
    return window.btoa(binary);
  }
}
