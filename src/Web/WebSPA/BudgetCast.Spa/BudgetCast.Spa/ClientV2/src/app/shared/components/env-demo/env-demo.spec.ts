import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EnvDemo } from './env-demo';

describe('EnvDemo', () => {
  let component: EnvDemo;
  let fixture: ComponentFixture<EnvDemo>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EnvDemo]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EnvDemo);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
