import { UserProfileDto } from './user-profile-dto';

export class UserRegistrationDto extends UserProfileDto {
  email: string = '';
  password: string = '';
  passwordConfirm: string = '';
}
