import { Injectable } from '@angular/core';
import { BaseService } from './base-data.service';

@Injectable({
  providedIn: 'root'
})
export class StorageService {

  private storage: any;

  constructor() { 
    this.storage = localStorage;
  }

  public setItem(key: string, value: string) {
    this.storage.setItem(key, value);
  }
    
  public getItem(key: string){ 
    return this.storage.getItem(key)
  }
  public removeItem(key:string) {
    this.storage.removeItem(key);
  }
  public clear(){
    this.storage.clear(); 
  }
}
