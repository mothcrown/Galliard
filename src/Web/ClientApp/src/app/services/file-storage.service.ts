import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class FileStorageService {

  file: File;

  constructor() { }

  addFile(file: File): void {
    this.file = file;
  }

  getFile(): File {
    return this.file;
  }

}
