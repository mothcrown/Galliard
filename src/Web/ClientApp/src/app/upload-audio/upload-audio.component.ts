import {Component, ElementRef} from '@angular/core';

@Component({
  selector: 'app-upload-audio',
  templateUrl: './upload-audio.component.html',
  styleUrls: ['./upload-audio.component.scss']
})

export class UploadAudioComponent {

  fileName: string;
  fileDragged: boolean = false;
  fileBeingDragged: boolean = false;

  onDragOver(event: any) {
    event.preventDefault();
    this.fileBeingDragged = true;
  }

  onDragLeave(event: any) {
    event.preventDefault();
    this.fileBeingDragged = false;
  }

  onDropSuccess(event: any) {
    event.preventDefault();
    this.onFileChange(event.dataTransfer.files);
  }

  onChange(event: any) {
    this.onFileChange(event.target.files);
  }

  private onFileChange(files: File[]) {
    // Let's ignore the other files for the moment, shall we?
    let file: File = files[0];
    this.fileName = file.name;
    this.fileDragged = true;
  }
}
