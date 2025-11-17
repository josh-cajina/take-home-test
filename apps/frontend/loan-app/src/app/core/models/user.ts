export interface AppUser {
  userId: string;
  email: string;
  roles: string[];
}

export interface AdminUserView {
  identityId: string;
  appUserId: string;
  email: string;
  firstName: string;
  lastName: string;
  roles: string[];
}
