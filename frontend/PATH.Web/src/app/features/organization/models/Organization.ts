import { OrgMember } from './OrgMember';
import { Project } from '../../project/models/Project';
import { ProjectMember } from '../../project/models/ProjectMember';

export interface Organization {
  id: string;
  name: string;
  createdById: string;
  createdAt: string;
  creatorName: string;
  currentUserRole: string;
  projects: Project[];
  members: OrgMember[];
}
