import { UserOrgPermissions } from "./UserOrgPermissions";

export interface UserOrgMembership {
  organizationId: string;
  userId: string;
  role: string;
  permissions: UserOrgPermissions;
}
