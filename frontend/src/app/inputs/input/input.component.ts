import {
  ChangeDetectionStrategy,
  Component,
  Input,
  OnInit,
} from '@angular/core';
import { AbstractControl, FormControl, FormGroup } from '@angular/forms';

type InputType = 'text' | 'password' | 'number';

@Component({
  selector: 'app-input',
  templateUrl: './input.component.html',
  styleUrls: ['./input.component.css'],
})
export class InputComponent implements OnInit {
  @Input() control!: FormControl;
  @Input() controlName!: string;

  @Input() placeholder: string = '';
  @Input() inputType: InputType = 'text';

  ngOnInit(): void {
    if (this.control == null || this.controlName == null) {
      throw 'Expected inputs not to be null!';
    }
  }
}
