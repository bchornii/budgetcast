export class ArrayExtensions {};

declare global {
    interface Array<T> {
      removeAt(idx: number): void;
    }
}
  
Array.prototype.removeAt = function (idx: number): void {
  let _self = this as Array<any>;
  _self.splice(idx, 1);
}