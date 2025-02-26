import {Component} from '@angular/core';
import { FileStorageService } from "../services/file-storage.service";
import { NovelizeClient, NovelizeCommand } from "../web-api-client";
import { ProcessStage } from "../process-stage/process-stage";

@Component({
  selector: 'app-novelize-process',
  templateUrl: './novelize-process.component.html',
  styleUrls: ['./novelize-process.component.scss']
})

export class NovelizeProcessComponent {

  stages: ProcessStage[] = [];

  constructor(
    private fileStorageService: FileStorageService,
    private novelizeClient: NovelizeClient
  ) {}

  ngOnInit() {
    this.sendFile();

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
          this.stages[0].message = "Audio uploaded";
          this.stages[0].loading = false;
          this.stages[0].success = true;
        },
        error => {
          console.error(error);
          this.stages[0].loading = false;
          this.stages[0].success = false;
        }
      );
    };
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
