export {};

declare global {
  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  interface Array<T> {
    removeAt(idx: number): void;
  }
}

Array.prototype.removeAt = function (idx: number): void {
  let _self = this as Array<any>;
  _self.splice(idx, 1);
};
