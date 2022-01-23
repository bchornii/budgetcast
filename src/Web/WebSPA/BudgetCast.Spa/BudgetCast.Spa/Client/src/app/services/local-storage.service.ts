import { Injectable } from '@angular/core';
import { BaseService } from './base-data.service';

@Injectable({
  providedIn: 'root'
})
export class LocalStorageService extends BaseService {

  constructor() { 
    super();
  }

  public setItem(key: string, value: string) {
    localStorage.setItem(key, value);
  }
    
  public getItem(key: string){ 
    return localStorage.getItem(key)
  }
  public removeItem(key:string) {
    localStorage.removeItem(key);
  }
  public clear(){
    localStorage.clear(); 
  }
}
